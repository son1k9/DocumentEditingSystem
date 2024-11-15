﻿using API.Domain.DocumentManagement.DocumentAggregate;
using API.Infrastructure.Data;
using API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Repositories.Implementations
{
	public class DocumentManagementRepository : IDocumentManagementRepository
	{
		private readonly ApplicationContext _context;

		public DocumentManagementRepository(ApplicationContext context)
		{
			_context = context;
		}

		private async Task<bool> SaveAsync()
		{
			try
			{
				await _context.SaveChangesAsync();
				return true;
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public async Task<bool> AddDocumentAsync(Document document)
		{
			await _context.Documents.AddAsync(document);

			return await SaveAsync();
		}

		public async Task<bool> UpdateDocumentAsync(Document document)
		{
			_context.Documents.Update(document);

			return await SaveAsync();
		}
		
		public async Task<bool> DeleteDocumentAsync(Document document)
		{
			_context.Documents.Remove(document);

			return await SaveAsync();
		}

		public async Task<List<Document>> GetAvailableDocumentsAsync(int ownerId)
		{
			return await _context.Documents.Where(p => p.OwnerId == ownerId).ToListAsync();
		}

		public async Task<Document> GetDocumentByIdAsync(int documentId)
		{
			return await _context.Documents.FirstOrDefaultAsync(x => x.Id == documentId); ;
		}
	}
}