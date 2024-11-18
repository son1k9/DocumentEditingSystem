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

		public async Task<bool> AddChangesAsync(List<Change> changes)
		{
			await _context.DocumentChanges.AddRangeAsync(changes);
			return await SaveAsync();
		}

		public async Task<List<Change>> GetChangesByDocumentAsync(int documentId)
		{
			return await _context.DocumentChanges.Where(p => p.DocumentId == documentId).ToListAsync();
		}
	}
}
