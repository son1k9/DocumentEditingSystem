using Domain.UserManagement.UserAggregate;

namespace API.Infrastructure.Repositories.Interfaces
{
	public interface IUserRepository
	{
		Task<bool> CreateAsync(User user);
		Task<bool> UpdateAsync(User user);
		Task<bool> DeleteAsync(User user);
		Task<User> GetByIdAsync(int id);
		Task<User> GetByUsernameAsync(string username);
		bool CheckIfCanEdit(int userID, int documentID);
	}
}
