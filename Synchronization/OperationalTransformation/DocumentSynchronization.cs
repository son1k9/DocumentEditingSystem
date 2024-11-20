using System.Diagnostics;

namespace OperationalTransformation;

public class DocumentSynchronization(int documentID, int version)
{
    public int DocumentID { get; } = documentID;

    readonly List<Operation> operations = [];
    readonly Dictionary<int, int> versionToIndex = [];

    // This should always be equal to last operation version + 1
    public int DocumentVersion{ get; private set; } = version;
    public int StartVersion { get; private set; } = version;

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
       
        operations.Add(operation);
        versionToIndex[DocumentVersion++] = operations.Count - 1;
        return (operation, DocumentVersion);
    }
}