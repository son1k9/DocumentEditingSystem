using Microsoft.AspNetCore.SignalR;
using OperationalTransformation;

namespace SignalR;

public class DocumentsHub(SynchronizationSystem synchronizationSystem, ILogger<DocumentsHub> logger) : Hub
{
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
        logger.LogInformation("Connection ended. ID:{ID}", connectionID);
        if (exception != null)
        {
            logger.LogError("With exception: {exception}", exception.ToString());
        }
        return base.OnDisconnectedAsync(exception);
    }

    public async Task JoinDocument(string documentID)
    {
        var connectionID = Context.ConnectionId;
        // TODO: check privileges first and check if document exists in db
        await Groups.AddToGroupAsync(connectionID, documentID);
        synchronizationSystem.AddDocument(documentID);
        logger.LogInformation("Connection {connectionID} added to group {documentID}", connectionID, documentID);
        await Clients.Caller.SendAsync("JoinedDocument", documentID);
    }

    public async Task SendOperation(Operation op, string documentID)
    {
        // TODO: check privileges first
        var (opToSend, nextVersion) = await synchronizationSystem.AddOperation(op, documentID);
        await Clients.Caller.SendAsync("ReceivedAcknowledge", nextVersion);
        await Clients.OthersInGroup(documentID).SendAsync("ReceivedOperation", opToSend, nextVersion);
    }
}
