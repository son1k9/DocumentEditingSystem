using API.Domain.ValueObjects;

namespace API.Infrastructure.Services.Interfaces
{
    public interface IDocumentManagementService
    {
        void LoadDocument(IFormFile documentFile, DocumentName documentName);
        void UpdateDocument(IFormFile newDocumentText, int documentId);
        void SetManager();
        void DeleteDocument();
    }

}
