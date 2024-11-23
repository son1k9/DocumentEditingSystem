using System.Diagnostics;

namespace OperationalTransformation;

public class DocumentSynchronization(int documentID, int version)
{
    public class OperationWithVersion(Operation operation, int version)
    {
        public Operation Operation { get; set; } = operation;
        public int Version { get; set; } = version;
    }

    public int DocumentID { get; } = documentID;

    readonly List<int> operationVersions = [];
    readonly List<Operation> operations = [];
    readonly Dictionary<int, int> versionToIndex = [];

    // This should always be equal to last operation version + 1
    public int DocumentVersion{ get; private set; } = version;
    
    public int StartVersion { get; private set; } = version;

    public List<OperationWithVersion> OperationsWithVersion()
    {
        Debug.Assert(operationVersions.Count == operations.Count);

        var count = operations.Count;
        var result = new List<OperationWithVersion>(count);
        for(int i = 0; i < count; ++i)
        {
            result.Add(new OperationWithVersion(operations[i], operationVersions[i]));
        }

        return result;
    }

    public IReadOnlyList<Operation> Operations => operations;

    public void Clear()
    {
        operations.Clear();
        versionToIndex.Clear();
        StartVersion = DocumentVersion;
    }

    public (Operation operationToSend, int newVersion) AddOperation(Operation operation, int version, IOperationsProvider operationsProvider)
    {
        Debug.Assert(operation.Type != OperationType.None);
        Debug.Assert(version <= DocumentVersion);


        // TODO: Test this
        if (version < StartVersion)
        {
            var (ops, lastVersion) = operationsProvider.GetOperations(DocumentID, version);
            Debug.Assert(ops.Count != 0);
            foreach(var op in ops)
            {
                operation = operation.Transform(op);
            }
            version = lastVersion;
        }

        if (version < DocumentVersion)
        {
            int opIndex = versionToIndex[version];

            for (int i = opIndex; i < operations.Count; i++)
            {
                operation = operation.Transform(operations[i]);
            }

            if (operation.Type == OperationType.None)
            {
                return (operation, DocumentVersion);
            }
        }

        // op.Version == DocumentVersion
        operationVersions.Add(DocumentVersion);
        operations.Add(operation);
        versionToIndex[DocumentVersion++] = operations.Count - 1;
        return (operation, DocumentVersion);
    }
}