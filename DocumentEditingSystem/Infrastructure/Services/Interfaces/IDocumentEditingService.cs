using API.Domain.Core.DocumentAggregate;
using API.Dtos.Read;
using API.Dtos.Write;

namespace API.Infrastructure.Services.Interfaces
{
    public interface IDocumentEditingService
    {
        Task<List<ChangeR>> GetDocumentChanges(int documentId, int userId);
        Task<List<ChangeR>> AddChangesToDocument(List<ChangeW> changes, int documentId, int userId);

        Task AddEditor(int documentId, int editorId, int userId);
		Task RemoveEditor(int documentId, int editorId, int userId);

	}
}
