using API.Domain.ValueObjects.Enums;
using API.Infrastructure.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OperationalTransformation;
using System.Diagnostics;

namespace API.Hubs;

[Authorize]
public class DocumentsHub(SynchronizationSystem synchronizationSystem,
                          ILogger<DocumentsHub> logger,
                          IDocumentManagementRepository documentsRepository,
                          IDocumentEditingRepository changesRepository) : Hub
{
    readonly static ReaderWriterLockSlim _lock = new();

    readonly static Dictionary<int, int> groupConnectionsCount = [];
    readonly static Dictionary<string, int> connectionGroup = [];
    readonly static Dictionary<int, FairLock> documentLock = [];

    readonly SynchronizationSystem synchronizationSystem = synchronizationSystem;
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

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var connectionID = Context.ConnectionId;

        _lock.EnterWriteLock();
        try
        {
            if (connectionGroup.Remove(connectionID, out int documentID))
            {
                logger.LogInformation("Connection {connectionID} removed from group {documentID}.", connectionID, documentID);

                groupConnectionsCount[documentID]--;

                Debug.Assert(groupConnectionsCount[documentID] >= 0);

                if (groupConnectionsCount[documentID] == 0)
                {
                    groupConnectionsCount.Remove(documentID);
                    documentLock.Remove(documentID);
                    synchronizationSystem.RemoveDocument(documentID);
                    logger.LogInformation("No connection left in group {documentID}. Group was removed", documentID);
                }
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        logger.LogInformation("Connection ended. ID:{ID}", connectionID);
        if (exception != null)
        {
            logger.LogError("With exception: {exception}.", exception.ToString());
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinDocument(int documentID)
    {
        var connectionID = Context.ConnectionId;

        // TODO: check privileges first and check if document exists in db

        bool connectionAlreadyInGroup = false;

        _lock.EnterWriteLock();
        try
        {
            connectionAlreadyInGroup = !connectionGroup.TryAdd(connectionID, documentID);

            if (!connectionAlreadyInGroup)
            {
                var version = await changesRepository.GetLastVersionForDocument(documentID);

                var documentAlreadyExist = !synchronizationSystem.AddDocument(documentID, version);

                if (!documentAlreadyExist)
                {
                    documentLock.Add(documentID, new FairLock());
                    groupConnectionsCount.Add(documentID, 0);
                }

                groupConnectionsCount[documentID]++;
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }

        if (connectionAlreadyInGroup)
        {
            logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                "Connection already in the group.", connectionID, documentID);

            await Clients.Caller.SendAsync("JoinedDocumentError", "Already connected to a document");

            return;
        }

        _lock.EnterReadLock();
        try
        {
            await documentLock[documentID].EnterAsync();
            try
            {
                var operations = synchronizationSystem.GetOperationsForDocument(documentID);
                var version = synchronizationSystem.GetVersionForDocument(documentID);
                await Groups.AddToGroupAsync(connectionID, documentID.ToString());
                await Clients.Caller.SendAsync("JoinedDocument", operations, version, documentID);
            }
            finally
            {
                documentLock[documentID].Exit();
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }

        logger.LogInformation("Connection {connectionID} added to group {documentID}.", connectionID, documentID);
    }

    public async Task SendOperation(Operation op, int version)
    {
        var connectionID = Context.ConnectionId;

        // TODO: check privileges first

        _lock.EnterReadLock();
        try
        {
            var documentID = connectionGroup[connectionID];
            await documentLock[documentID].EnterAsync();
            try
            {
                var (opToSend, nextVersion) = synchronizationSystem.AddOperation(op, version, documentID, operationsProvider);
                await Clients.Caller.SendAsync("ReceivedAcknowledge", nextVersion);
                await Clients.OthersInGroup(documentID.ToString()).SendAsync("ReceivedOperation", opToSend, nextVersion);
            }
            finally
            {
                documentLock[documentID].Exit();
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }
}
