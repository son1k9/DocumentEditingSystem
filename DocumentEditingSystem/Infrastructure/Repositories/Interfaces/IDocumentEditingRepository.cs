using API.Domain.Core.DocumentAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IDocumentEditingRepository
	{
		Task<bool> AddChangesAsync(List<Change> changes);
		Task<List<Change>> GetChangesByDocumentAsync(int documentId);
	}
}
