using API.Domain.Core.DocumentAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IDocumentEditingRepository
	{
		Task AddDocumentAsync(EditingDocument editingDocument);
		Task<bool> UpdateDocumentAsync(EditingDocument editingDocument);
		Task<EditingDocument> GetDocumentByIdAsync(int documentId);
		Task<List<EditingDocument>> GetAvailableDocuments(int userId);
	}
}
