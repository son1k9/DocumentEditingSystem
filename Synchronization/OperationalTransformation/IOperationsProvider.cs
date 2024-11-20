namespace OperationalTransformation;
public interface IOperationsProvider
{
    (List<Operation>, int) GetOperations(int documentID, int verstionStartFrom);
}
