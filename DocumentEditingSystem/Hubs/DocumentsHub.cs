using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using OperationalTransformation;
using System.Collections.Concurrent;

namespace API.Hubs
{
    [Authorize]
    public class DocumentsHub(SynchronizationSystem synchronizationSystem, ILogger<DocumentsHub> logger) : Hub
    {
        // TODO: Remove documents when all users of a group disconect

        readonly static ConcurrentDictionary<string, string> connectionGroup = [];

        readonly SynchronizationSystem synchronizationSystem = synchronizationSystem;
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

            if (connectionGroup.TryRemove(connectionID, out string? documentID))
            {
                logger.LogInformation("Connection {connectionID} removed from group {documentID}.", connectionID, documentID);
            }

            logger.LogInformation("Connection ended. ID:{ID}", connectionID);
            if (exception != null)
            {
                logger.LogError("With exception: {exception}.", exception.ToString());
            }
            return base.OnDisconnectedAsync(exception);
        }

        public async Task JoinDocument(string documentID)
        {
            var connectionID = Context.ConnectionId;
            var connectionAlreadyInGroup = !connectionGroup.TryAdd(connectionID, documentID);

            if (connectionAlreadyInGroup)
            {
                logger.LogInformation("Failed to add connection {connectionID} to group {documentID}." +
                    "Connection already in the group.", connectionID, documentID);
                await Clients.Caller.SendAsync("JoinedDocumentError", "Already connected to a document");
                return;
            }

            // TODO: check privileges first and check if document exists in db
            await Groups.AddToGroupAsync(connectionID, documentID);
            synchronizationSystem.AddDocument(documentID);
            logger.LogInformation("Connection {connectionID} added to group {documentID}.", connectionID, documentID);
            await Clients.Caller.SendAsync("JoinedDocument", documentID);
        }

        public async Task SendOperation(Operation op)
        {
            // TODO: check privileges first
            var connectionID = Context.ConnectionId;
            var documentID = connectionGroup[connectionID];
            var (opToSend, nextVersion) = await synchronizationSystem.AddOperation(op, documentID);
            await Clients.Caller.SendAsync("ReceivedAcknowledge", nextVersion);
            await Clients.OthersInGroup(documentID).SendAsync("ReceivedOperation", opToSend, nextVersion);
        }
    }
}
