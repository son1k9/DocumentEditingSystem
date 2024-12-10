using API.Domain.Core.DocumentAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IDocumentEditingRepository
	{
		Task<bool> AddChangesAsync(List<Change> changes);
		List<Change> GetChangesByDocument(int documentId);
        List<Change> GetChangesByDocumentStartFromVersion(int documentId, int versionStartFrom);
        Task<int> GetLastVersionForDocument(int documentID);
	}
}
