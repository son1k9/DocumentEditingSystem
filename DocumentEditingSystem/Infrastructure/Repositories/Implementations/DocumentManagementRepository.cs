using API.Domain.DocumentManagement.DocumentAggregate;
using API.Infrastructure.Data;
using API.Infrastructure.Repositories.Interfaces;
using Domain.UserManagement.UserAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Repositories.Implementations
{
	public class DocumentManagementRepository : IDocumentManagementRepository
	{
		private readonly ApplicationContext _context;
		private readonly IMediator _mediator;

		public DocumentManagementRepository(ApplicationContext context, IMediator mediator)
		{
			_context = context;
			_mediator = mediator;
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
                Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> AddDocumentAsync(Document document)
		{
			await _context.Documents.AddAsync(document);

			var result = await SaveAsync();

			return result;
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

		public async Task<List<Document>> GetAvailableDocumentsAsync(int userId)
		{
			return await _context.Documents.Where(p => p.OwnerId == userId || p.Editors.Any(e => e.Id == userId)).Include(u => u.Editors).ToListAsync();
		}

		public async Task<Document?> GetDocumentByIdAsync(int documentId)
		{
			return await _context.Documents.Include(u => u.Editors).FirstOrDefaultAsync(x => x.Id == documentId);
		}

		public async Task<Document?> GetDocumentWithContentByIdAsync(int documentId)
		{
			return await _context.Documents.Include(u => u.Editors).Include(d => d.Content).FirstOrDefaultAsync(x => x.Id == documentId);
		}
    }
}
