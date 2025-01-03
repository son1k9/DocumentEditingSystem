﻿using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IDocumentManagementRepository
	{
		Task<bool> AddDocumentAsync(Document document);

		Task<bool> UpdateDocumentAsync(Document document);

		Task<bool> DeleteDocumentAsync(Document document);

		Task<Document?> GetDocumentByIdAsync(int id);
		
		Task<Document?> GetDocumentWithContentByIdAsync(int id);

		Task<List<Document>> GetAvailableDocumentsAsync(int userId);
	}
}
