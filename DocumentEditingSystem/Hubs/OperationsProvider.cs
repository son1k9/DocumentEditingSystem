using API.Domain.ValueObjects.Enums;
using API.Infrastructure.Repositories.Interfaces;
using OperationalTransformation;

namespace API.Hubs;

public class OperationsProvider(IDocumentEditingRepository changesRepository) : IOperationsProvider
{
    readonly IDocumentEditingRepository changesRepository = changesRepository;

    public (List<Operation>, int) GetOperations(int documentID, int verstionStartFrom)
    {
        var changes = changesRepository.GetChangesByDocumentStartFromVersion(documentID, verstionStartFrom);

        if (changes.Count == 0)
        {
            return ([], 0); 
        }

        static OperationType ChangeTypeToOperationType(ChangeType changeType)
        {
            return changeType switch
            {
                ChangeType.Add => OperationType.Insert,
                ChangeType.Delete => OperationType.Delete,
                _ => throw new ArgumentException(nameof(changeType))
            };
        }

        var operations = new List<Operation>(changes.Count);
        foreach (var c in changes)
        {
            var operation = new Operation
            {
                Type = ChangeTypeToOperationType(c.ChangeType),
                Pos = c.ChangePosition,
                Text = c.Text,
                UserID = c.EditorId
            };
            operations.Add(operation);
        }

        return (operations, changes[^1].Version + 1);
    }
}