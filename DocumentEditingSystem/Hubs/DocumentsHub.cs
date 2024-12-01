using API.Domain.Core.DocumentAggregate;
using API.Domain.ValueObjects.Enums;
using API.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OperationalTransformation;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace API.Hubs;

[Authorize]
public class DocumentsHub(ILogger<DocumentsHub> logger,
                          IUserRepository usersRepository,
                          IDocumentManagementRepository documentsRepository,
                          IDocumentEditingRepository changesRepository) : Hub
{
    class DocumentData(FairLock _lock, int count, DocumentSynchronization document)
    {
        public FairLock Lock { get; } = _lock;
        int _connectionCount = count;
        public int ConnectionsCount => _connectionCount;
        public DocumentSynchronization Document = document;

        public void IncrementCount()
        {
            Interlocked.Increment(ref _connectionCount);
        }

        public void DecrementCount()
        {
            Interlocked.Decrement(ref _connectionCount);
            Debug.Assert(ConnectionsCount > 0);
        }
    }

    readonly static ConcurrentDictionary<string, int> connectionGroup = [];
    readonly static ConcurrentDictionary<int, DocumentData> documents = [];

    readonly IUserRepository usersRepository = usersRepository;
    readonly IDocumentManagementRepository documentsRepository = documentsRepository;
    readonly IDocumentEditingRepository changesRepository = changesRepository;
    readonly IOperationsProvider operationsProvider = new OperationsProvider(changesRepository);

    readonly ILogger<DocumentsHub> logger = logger;

    public override Task OnConnectedAsync()
    {
        var connectionID = Context.ConnectionId;
        logger.LogInformation("New connection. ID:{ID}", connectionID);
        return base.OnConnectedAsync();
    }

    public async override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionID = Context.ConnectionId;

        if (connectionGroup.Remove(connectionID, out int documentID))
        {
            logger.LogInformation("Connection {connectionID} removed from group {documentID}.", connectionID, documentID);

            documents[documentID].DecrementCount();

            if (documents[documentID].ConnectionsCount <= 0)
            {
                documents.Remove(documentID, out var document);

                Debug.Assert(document != null);

                await document.Lock.EnterAsync();
                try
                {
                    await SaveDocument(document.Document, documentID);
                }
                finally
                {
                    document.Lock.Exit();
                }

                logger.LogInformation("No connection left in group {documentID}. Group was removed", documentID);
            }
        }

        logger.LogInformation("Connection ended. ID:{ID}", connectionID);
        if (exception != null)
        {
            logger.LogError("With exception: {exception}.", exception.ToString());
        }

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinDocument(int documentID, int clientVersion)
    {
        var connectionID = Context.ConnectionId;

        if (await documentsRepository.GetDocumentByIdAsync(documentID) == null)
        {
            logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                "Document does not exist.", connectionID, documentID);

            await Clients.Caller.SendAsync("JoinDocumentError", "Document does not exist");

            return;
        }

        int userID = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var canEdit = usersRepository.CheckIfCanEdit(userID, documentID);
        if (!canEdit)
        {
            logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                "User does not have needed privileges.", connectionID, documentID);

            await Clients.Caller.SendAsync("JoinDocumentError", "User does not have needed privileges");

            return;
        }

        var connectionAlreadyInGroup = !connectionGroup.TryAdd(connectionID, documentID);
        if (connectionAlreadyInGroup)
        {
            logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                "Connection already in the group.", connectionID, documentID);

            await Clients.Caller.SendAsync("JoinDocumentError", "Already connected to a document");

            return;
        }

        var documentAlreadyExist = documents.ContainsKey(documentID);
        if (!documentAlreadyExist)
        {
            await AddDocument(documentID);
        }

        var document = documents[documentID];
        document.IncrementCount();
        await document.Lock.EnterAsync();
        try
        {
            var (operationsFromDb, _) = operationsProvider.GetOperations(documentID, clientVersion);
            var currentOperations = document.Document.Operations;
            foreach (var op in currentOperations)
            {
                operationsFromDb.Add(op);
            }

            var version = document.Document.DocumentVersion;
            await Groups.AddToGroupAsync(connectionID, documentID.ToString());
            await Clients.Caller.SendAsync("JoinedDocument", operationsFromDb, version, documentID);
        }
        finally
        {
            document.Lock.Exit();
        }

        logger.LogInformation("Connection {connectionID} added to group {documentID}.", connectionID, documentID);
    }

    public async Task SendOperation(Operation op, int version)
    {
        var connectionID = Context.ConnectionId;

        if (connectionGroup.TryGetValue(connectionID, out var documentID))
        {
            await Clients.Caller.SendAsync("SendOperationError", "User has no group.");

            return;
        }

        int userID = int.Parse(Context.User!.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var canEdit = usersRepository.CheckIfCanEdit(userID, documentID);
        if (!canEdit)
        {
            logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                "User does not have needed privileges.", connectionID, documentID);

            await Clients.Caller.SendAsync("SendOperationError", "User does not have needed privileges");

            return;
        }

        var document = documents[documentID];
        await document.Lock.EnterAsync();
        try
        {
            var (opToSend, nextVersion) = document.Document.AddOperation(op, version, operationsProvider);
            await Clients.Caller.SendAsync("ReceivedAcknowledge", nextVersion);
            await Clients.OthersInGroup(documentID.ToString()).SendAsync("ReceivedOperation", opToSend, nextVersion);
        }
        finally
        {
            document.Lock.Exit();
        }
    }

    private async Task SaveDocument(DocumentSynchronization documentSynchronization, int documentID)
    {
        var operations = documentSynchronization.OperationsWithVersion();

        static ChangeType OperationTypeToChangeType(OperationType operationType)
        {
            return operationType switch
            {
                OperationType.Insert => ChangeType.Add,
                OperationType.Delete => ChangeType.Delete,
                _ => throw new ArgumentException(nameof(operationType))
            };
        }

        var changes = new List<Change>(operations.Count);
        foreach (var opv in operations)
        {
            var op = opv.Operation;
            var change = new Change(op.UserID, documentID, op.Pos, op.Text, OperationTypeToChangeType(op.Type), opv.Version);
            changes.Add(change);
        }

        var document = await documentsRepository.GetDocumentWithContentByIdAsync(documentID);
        if (document == null)
        {
            logger.LogInformation("Failed to save document with ID:{documentID}. Document does not exist.", documentID);
            return;
        }

        await changesRepository.AddChangesAsync(changes);

        var newText = DocumentSynchronization.UpdateDocument(document.Content.Text, documentSynchronization.Operations);
        
        var version = documentSynchronization.DocumentVersion;
        document.UpdateDocument(newText, version);
        await documentsRepository.UpdateDocumentAsync(document);

        documentSynchronization.Clear();
    }

    private async Task AddDocument(int documentID)
    {
        var version = await changesRepository.GetLastVersionForDocument(documentID);
        var documentAdded = documents.TryAdd(documentID, new DocumentData(new FairLock(), 0, new DocumentSynchronization(documentID, version)));
        Debug.Assert(documentAdded);
    }
}
