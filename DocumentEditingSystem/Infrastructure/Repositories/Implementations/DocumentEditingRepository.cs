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

		public List<Change> GetChangesByDocument(int documentId, int versionStartFrom)
		{
			var query = _context.DocumentChanges.Where(c => c.DocumentId == documentId);

			if (versionStartFrom != -1)
			{
				query.Where(c => c.Version >= versionStartFrom);
			}

			return query.OrderBy(c => c.Version).ToList();
		}

        public async Task<int> GetLastVersionForDocument(int documentID)
        {
            var lastChange = await _context.DocumentChanges.Where(c => c.DocumentId == documentID).OrderByDescending(c => c.Version).FirstOrDefaultAsync();

			if (lastChange == null)
			{
				return 0;
			}

			return lastChange.Version + 1;
        }
    }
}
