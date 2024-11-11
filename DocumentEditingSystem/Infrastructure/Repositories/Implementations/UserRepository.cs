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

		public async Task<bool> Create(User user)
		{
			await _context.AddAsync(user);

			return await SaveAsync();
		}

		public async Task<bool> Delete(User user)
		{
			_context.Users.Remove(user);
			return await SaveAsync();
		}

		public async Task<User> GetByUsername(string username)
		{
			return await _context.Users.FirstOrDefaultAsync(p => p.Username.Value == username);
		}

		public async Task<User> GetById(int id)
		{
			return await _context.Users.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<bool> Update(User user)
		{
			_context.Users.Update(user);
			return await SaveAsync();
		}

	}
}
