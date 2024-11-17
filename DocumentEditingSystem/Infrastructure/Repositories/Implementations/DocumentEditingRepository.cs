using API.Domain.Core.DocumentAggregate;
using API.Infrastructure.Data;
using API.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Repositories.Implementations
{
	public class DocumentEditingRepository : IDocumentEditingRepository
	{
		private readonly ApplicationContext _context;
		public DocumentEditingRepository(ApplicationContext context) 
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

		public async Task AddDocumentAsync(EditingDocument editingDocument)
		{
			_context.EditingDocuments.Add(editingDocument);
			await SaveAsync();
		}

		public async Task<bool> UpdateDocumentAsync(EditingDocument editingDocument)
		{
			_context.EditingDocuments.Update(editingDocument);
			return await SaveAsync();
		}

		public async Task<EditingDocument> GetDocumentByIdAsync(int documentId)
		{
			return await _context.EditingDocuments.FirstOrDefaultAsync(x => x.Id == documentId);
		}

		public async Task<List<EditingDocument>> GetAvailableDocuments(int userId)
		{
			return await _context.EditingDocuments.Where(p => p.OwnerId == userId).ToListAsync();
		}
	}
}
