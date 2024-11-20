using System.Diagnostics;

namespace OperationalTransformation.Test;

public class DocumentSynchronizationClient
{
    readonly List<Operation> receivedOperations = [];
    readonly List<Operation> operations;
    public IReadOnlyList<Operation> Operations => operations;

    int documentVersion = 0;

    int lastSentOperationIndex = -1;
    int currentSentOperationIndex = -1;

    string document = "";

    public DocumentSynchronizationClient(List<Operation> operations)
    {
        this.operations = operations;
        foreach (var op in operations)
        {
            document = document.ApplyOp(op);
        }
    }

    public string Document => document;

    public (Operation operation, int version)?  SendOperation()
    {
        Debug.Assert(currentSentOperationIndex == -1);

        if (lastSentOperationIndex + 1 < operations.Count)
        {
            currentSentOperationIndex = ++lastSentOperationIndex;
            var op = operations[currentSentOperationIndex];
            return (op, documentVersion);
        }

        return null;
    }

    public void ReceiveOperation(Operation op, int newVersion)
    {
        var oldOp = op;
        int i = lastSentOperationIndex;

        if (currentSentOperationIndex == -1)
        {
            i++;
        }

        for (; i < operations.Count; i++)
        {
            op = op.Transform(operations[i], true);
            operations[i] = operations[i].Transform(oldOp);
        }
        document = document.ApplyOp(op);
        receivedOperations.Add(op);
        documentVersion = newVersion;
    }

    public void ReceiveAcknowledge(int newVersion)
    {
        documentVersion = newVersion;
        currentSentOperationIndex = -1;
    }
}

public class DocumentSynchronizationTest
{
    [Fact]
    public void AddOperation_Test()
    {
        var syncSystem = new DocumentSynchronization(0, 0);

        var client1 = new DocumentSynchronizationClient([Operation.CreateInsertOp(0, "text"), Operation.CreateDeleteOp(2, "xt")]);
        var client2 = new DocumentSynchronizationClient([Operation.CreateInsertOp(0, "word"), Operation.CreateInsertOp(4, " another abcd")]);
        var client3 = new DocumentSynchronizationClient([Operation.CreateInsertOp(0, "the"), Operation.CreateDeleteOp(0, "the")]);

        List<DocumentSynchronizationClient> clients = [client1, client2, client3];

        void SimulateCommunication(List<DocumentSynchronizationClient> clients, DocumentSynchronization syncSystem)
        {
            List<(Operation operationToSend, int newVersion)> results = [];
            for (int i = 0; i < clients.Count; i++)
            {
                DocumentSynchronizationClient client = clients[i];
                var send = client.SendOperation()!;
                results.Add(syncSystem.AddOperation(send.Value.operation, send.Value.version, null));
            }

            for (int i = 0; i < clients.Count; i++)
            {
                DocumentSynchronizationClient client = clients[i];
                for (int j = 0; j < clients.Count; j++)
                {
                    var (op, ver) = results[j];
                    if (i == j)
                    {
                        client.ReceiveAcknowledge(ver);
                        continue;
                    }

                    client.ReceiveOperation(op, ver);
                }
            }
        }

        SimulateCommunication(clients, syncSystem);
        SimulateCommunication(clients, syncSystem);

        string result = "";
        foreach (var op in syncSystem.Operations)
        {
            result = result.ApplyOp(op);
        }

        Assert.Equal(client1.Document, client2.Document);
        Assert.Equal(client2.Document, client3.Document);
        Assert.Equal(client2.Document, client3.Document);
        Assert.Equal(result, client1.Document);
    }
}