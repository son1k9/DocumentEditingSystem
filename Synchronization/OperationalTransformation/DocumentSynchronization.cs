using System.Diagnostics;

namespace OperationalTransformation;

public class DocumentSynchronization
{
    readonly List<Operation> operations = [];
    readonly Dictionary<int, int> versionToIndex = [];

    // This should always be equal to last operation version + 1
    int documentVersion = 0;

    public int Version => documentVersion;

    public IReadOnlyList<Operation> Operations => operations;

    public (Operation operationToSend, int newVersion) AddOperation(Operation op, int version)
    {
        Debug.Assert(op.Type != OperationType.None);
        Debug.Assert(version <= documentVersion);

        if (version < documentVersion)
        {
            int opIndex = versionToIndex[version];

            for (int i = opIndex; i < operations.Count; i++)
            {
                op = op.Transform(operations[i]);
            }

            if (op.Type == OperationType.None)
            {
                return (op, documentVersion);
            }
        }

        // op.Version == documentVersion
       
        operations.Add(op);
        versionToIndex[documentVersion++] = operations.Count - 1;
        return (op, documentVersion);
    }
}