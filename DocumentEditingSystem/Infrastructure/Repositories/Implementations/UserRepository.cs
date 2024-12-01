using API.Infrastructure.Data;
using API.Infrastructure.Repositories.Interfaces;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace API.Infrastructure.Repositories.Implementations
{
	public class UserRepository : IUserRepository
	{
		private readonly ApplicationContext _context;

		public UserRepository(ApplicationContext context)
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

		public async Task<bool> CreateAsync(User user)
		{
			await _context.AddAsync(user);

			return await SaveAsync();
		}

		public async Task<bool> DeleteAsync(User user)
		{
			_context.Users.Remove(user);
			return await SaveAsync();
		}

		public async Task<User> GetByUsernameAsync(string username)
		{
			return await _context.Users.FirstOrDefaultAsync(p => p.Username.Value == username);
		}

		public async Task<User> GetByIdAsync(int id)
		{
			return await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<bool> UpdateAsync(User user)
		{
			_context.Users.Update(user);
			return await SaveAsync();
		}

		public bool CheckIfCanEdit(int userID, int documentID)
		{
			var query = _context.Documents.Where(d => d.Id == documentID && d.Editors.Any(u => u.Id == userID));
			if (query.Any())
			{
				return true;
			}
			return false;
		}
	}
}
