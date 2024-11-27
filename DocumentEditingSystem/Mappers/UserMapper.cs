using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using API.Dtos.Read;
using API.Dtos.Write;
using Domain.UserManagement.UserAggregate;

namespace API.Mappers
{
	public static class UserMapper
	{
		public static User DtoToUser(UserW userW)
		{
			Name name = new Name(userW.FirstName, userW.LastName);
			Email email = new Email(userW.Email);
			PhoneNumber phoneNumber = new PhoneNumber(userW.PhoneNumber);
			Username username = new Username(userW.Username);
			Password password = new Password(userW.Password);
			User user = new User(name, username, email, phoneNumber, password);
			return user;
		}

		public static UserR UserToDto(User user)
		{
			UserR userR = new UserR
			{
				Id = user.Id,
				FirstName = user.Name.FirstName,
				LastName = user.Name.LastName,
				Username = user.Username.Value,
				Email = user.Email.Value,
				PhoneNumber = user.PhoneNumber.Value
			};
			return userR;
		}
	}
}
