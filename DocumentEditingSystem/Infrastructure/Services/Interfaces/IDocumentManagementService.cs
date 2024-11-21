using API.Domain.DocumentManagement.DocumentAggregate;
using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using API.Dtos.Read;
using API.Dtos.Write;
using Microsoft.AspNetCore.Mvc;

namespace API.Infrastructure.Services.Interfaces
{
    public interface IDocumentManagementService
    {
        Task LoadDocument(IFormFile documentFile, int userId, Username username);
        Task UpdateDocument(string text, int userId, int documentId);
        Task<List<DocumentR>> GetAvailableDocuments(int userId);
        Task DeleteDocument(int documentId, int userId);
        Task<DocumentR> GetDocumentById(int documentId, int userId);
    }

}
