using API.Domain.DocumentManagement.DocumentAggregate;
using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Repositories.Interfaces;
using API.Infrastructure.Services.Interfaces;
using API.Mappers;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

namespace API.Infrastructure.Services.Implementations
{
    public class DocumentManagementService : IDocumentManagementService
    {
        private readonly IDocumentManagementRepository _documentManagementRepository;

        public DocumentManagementService(IDocumentManagementRepository documentManagementRepository)
        {
            _documentManagementRepository = documentManagementRepository;
        }

		public async Task DeleteDocument(int documentId, int userId)
		{
			Document document = await _documentManagementRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException("Document was not found");
            }

            if (document.OwnerId != userId)
            {
                throw new ArgumentException("Document was not found");
            }

            await _documentManagementRepository.DeleteDocumentAsync(document);
		}

		public async Task<List<DocumentR>> GetAvailableDocuments(int userId)
		{
			List<Document> availableDocuments = await _documentManagementRepository.GetAvailableDocumentsAsync(userId);
            List<DocumentR> documentsR = new List<DocumentR>();

            foreach (var document in availableDocuments)
            {
                documentsR.Add(DocumentMapper.DocumentToDto(document));
            }

            return documentsR;
		}

		public async Task LoadDocument(IFormFile documentFile, int userId, Username username)
        {
            string text;

            using (var reader = new StreamReader(documentFile.OpenReadStream()))
            {
                text = await reader.ReadToEndAsync();
            }

            DocumentName documentName = new DocumentName(documentFile.FileName);
            Document document = new Document(documentName, text);
            document.SetOwnerId(userId);

            var result = await _documentManagementRepository.AddDocumentAsync(document);

            if (!result)
            {
                throw new Exception("Error adding to the database!");
            }

        }

        public async Task<DocumentR> UpdateDocument(IFormFile newDocumentText, int userId, int documentId)
        {
            Document document = await _documentManagementRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                throw new ArgumentException("Document was not fount");
            }

            if (document.OwnerId != userId)
            {
                throw new ArgumentException("Document was not found");
            }

            string newText;

			using (var reader = new StreamReader(newDocumentText.OpenReadStream()))
			{
				newText = await reader.ReadToEndAsync();
			}

            document.UpdateDocument(newText);

			await _documentManagementRepository.UpdateDocumentAsync(document);

            return DocumentMapper.DocumentToDto(document);
        }
    }
}
