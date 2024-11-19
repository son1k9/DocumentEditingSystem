namespace OperationalTransformation;

public class SynchronizationSystem
{
    readonly Dictionary<string, DocumentSynchronization> documents = [];

    public (Operation operationToSend, int newVersion) AddOperation(Operation op, int version, string documentID)
    {
        return documents[documentID].AddOperation(op, version);
    }

    public bool AddDocument(string documentID)
    {
        if (!documents.ContainsKey(documentID))
        {
            documents.Add(documentID, new DocumentSynchronization());
            return true;
        }
        return false;
    }

    public int GetVersionForDocument(string documentID)
    {
        return documents[documentID].Version;
    }

    public IReadOnlyList<Operation> GetOperationsForDocument(string documentID)
    {
        return documents[documentID].Operations;
    }

    public void RemoveDocument(string documentID)
    {
        documents.Remove(documentID);
    }
}