using System.Diagnostics;

namespace OperationalTransformation;

public class DocumentSynchronization
{
    readonly List<Operation> operations = [];

    public IReadOnlyList<Operation> Operations => operations;

    // This should always be equal to last operation version + 1
    int documentVersion = 0;

    public (Operation operationToSend, int newVersion) AddOperation(Operation op)
    {
        Debug.Assert(op.Type != OperationType.None);
        Debug.Assert(op.Version <= documentVersion);

        if (op.Version == documentVersion)
        {
            documentVersion++;           
            operations.Add(op);
            return (op, documentVersion);
        }

        // operation.Version < DocumentVersion
        int opIndex = operations.Count - 1;
        while (opIndex >= 0 && op.Version < operations[opIndex].Version)
        {
            opIndex--;
        }

        for(int i = opIndex; i < operations.Count; i++)
        {
            op = op.Transform(operations[i]);
        }

        if (op.Type == OperationType.None)
        {
            return (op, documentVersion);
        }

        op.Version = documentVersion++;           
        operations.Add(op);
        return (op, documentVersion);
    }
}