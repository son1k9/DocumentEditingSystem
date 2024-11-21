using API.Domain.Core.DocumentAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IDocumentEditingRepository
	{
		Task<bool> AddChangesAsync(List<Change> changes);
		List<Change> GetChangesByDocument(int documentId, int versionStartFrom = -1);
		Task<int> GetLastVersionForDocument(int documentID);
	}
}
