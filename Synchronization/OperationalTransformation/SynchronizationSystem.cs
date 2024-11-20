namespace OperationalTransformation;

public class SynchronizationSystem
{
    readonly Dictionary<int, DocumentSynchronization> documents = [];

    public (Operation operationToSend, int newVersion) AddOperation(Operation op, int version, int documentID, IOperationsProvider operationsProvider)
    {
        return documents[documentID].AddOperation(op, version, operationsProvider);
    }

    public bool AddDocument(int documentID, int version)
    {
        if (!documents.ContainsKey(documentID))
        {
            documents.Add(documentID, new DocumentSynchronization(documentID, version));
            return true;
        }
        return false;
    }

    public void ClearDocument(int documentID)
    {
        documents[documentID].Clear();
    }

    public int GetVersionForDocument(int documentID)
    {
        return documents[documentID].DocumentVersion;
    }

    public IReadOnlyList<Operation> GetOperationsForDocument(int documentID)
    {
        return documents[documentID].Operations;
    }

    public void RemoveDocument(int documentID)
    {
        documents.Remove(documentID);
    }
}