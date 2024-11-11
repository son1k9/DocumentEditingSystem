using Domain.UserManagement.UserAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> Create(User user);
		Task<bool> Update(User user);
		Task<bool> Delete(User user);
		Task<User> GetById(int id);
		Task<User> GetByUsername(string username);
	}
}
