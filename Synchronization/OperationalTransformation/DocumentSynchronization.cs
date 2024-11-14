using System.Diagnostics;

namespace OperationalTransformation;

public class DocumentSynchronization
{
    readonly List<Operation> operations = [];
    readonly Dictionary<int, int> versionToIndex = [];

    // This should always be equal to last operation version + 1
    int documentVersion = 0;

    public IReadOnlyList<Operation> Operations => operations;

    public (Operation operationToSend, int newVersion) AddOperation(Operation op)
    {
        Debug.Assert(op.Type != OperationType.None);
        Debug.Assert(op.Version <= documentVersion);

        if (op.Version < documentVersion)
        {
            int opIndex = versionToIndex[op.Version];

            for (int i = opIndex; i < operations.Count; i++)
            {
                op = op.Transform(operations[i]);
            }

            if (op.Type == OperationType.None)
            {
                return (op, documentVersion);
            }

            op.Version = documentVersion;
        }

        // op.Version == documentVersion

        documentVersion++;           
        operations.Add(op);
        versionToIndex[op.Version] = operations.Count - 1;
        return (op, documentVersion);
    }
}