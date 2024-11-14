namespace OperationalTransformation;

public class SynchronizationSystem
{
    readonly ReaderWriterLockSlim _lock = new();
    readonly Dictionary<string, DocumentSynchronization> documents = [];
    readonly Dictionary<string, FairLock> locks = [];

    public async Task<(Operation operationToSend, int newVersion)> AddOperation(Operation op, string documentID)
    {
        _lock.EnterReadLock();
        try
        {
            await locks[documentID].EnterAsync();
            (Operation, int) result;
            try
            {
                result = documents[documentID].AddOperation(op);
            }
            finally
            {
                locks[documentID].Exit();
            }
            return result;
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    public void AddDocument(string documentID)
    {
        _lock.EnterWriteLock();
        try
        {
            if (!documents.ContainsKey(documentID))
            {
                documents.Add(documentID, new DocumentSynchronization());
                locks.Add(documentID, new FairLock());
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    public void RemoveDocument(string documentID)
    {
        _lock.EnterWriteLock();
        try
        {
            documents.Remove(documentID);
            locks.Remove(documentID);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }
}