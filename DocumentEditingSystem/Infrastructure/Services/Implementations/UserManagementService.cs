using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;
using API.Dtos.Read;
using API.Dtos.Write;
using API.Infrastructure.Repositories.Interfaces;
using API.Infrastructure.Services.Interfaces;
using API.Mappers;
using API.TokenConfig;
using Domain.UserManagement.UserAggregate;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Infrastructure.Services.Implementations
{
    public class UserManagementService : IUserManagementService
    {
        private readonly IUserRepository _userRepository;

        public UserManagementService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        private string GenerateToken(Username username, Role role)
        {
			var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username.Value ),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, role.ToString())                         
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity
                (
                claims, 
                "Token", 
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType
                );

			var jwt = new JwtSecurityToken(
				issuer: AuthOptions.ISSUER,
				audience: AuthOptions.AUDIENCE,
				claims: claimsIdentity.Claims,
				expires: DateTime.UtcNow.Add(TimeSpan.FromDays(30)),
				signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
		}

        public async Task<string> Autorize(Username username, Password password)
        {
            User user = await _userRepository.GetByUsername(username.Value);
			
            if(user == null)
            {
                throw new ArgumentException("Invalid login");
            }


            var jwt = GenerateToken(username, user.Role);
            return jwt;
		}

        public async Task<UserR> ChangeName(Name name, Username username)
        {
            User user = await _userRepository.GetByUsername(username.Value);

            if(user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            user.ChangeName(name);

            bool result = await _userRepository.Update(user);

            if (!result)
            {
				throw new Exception("An error occurred updating the data");
			}

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> ChangeEmail(Email email, Username username)
        {
            User user = await _userRepository.GetByUsername(username.Value);

            if(user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            user.ChangeEmail(email);
            bool result = await _userRepository.Update(user);
            if (!result)
            {
				throw new Exception("An error occurred updating the data");
			}

            return UserMapper.UserToDto(user);
        }

        public async Task<UserR> ChangePassword(Password newPassword, Password oldPassword, Username username)
        {
			User user = await _userRepository.GetByUsername(username.Value);

			if (user == null)
			{
				throw new ArgumentException("The user does not exist");
			}

            var checkResult = user.Password.Compare(oldPassword);

            if (!checkResult)
            {
                throw new UnauthorizedAccessException("Old password is not valid");
            }

			user.ChangePassword(newPassword);
			bool result = await _userRepository.Update(user);

			if (!result)
			{
				throw new Exception("An error occurred updating the data");
			}

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> ChangePhoneNumber(PhoneNumber newNumber, Username username)
        {
			User user = await _userRepository.GetByUsername(username.Value);

			if (user == null)
			{
				throw new ArgumentException("The user does not exist");
			}

			user.ChangePhoneNumber(newNumber);
			bool result = await _userRepository.Update(user);

			if (!result)
			{
				throw new Exception("An error occurred updating the data");
			}

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> GetUserData(Username username)
        {
            User user = await _userRepository.GetByUsername(username.Value);
            if (user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            UserR userR = UserMapper.UserToDto(user);
            return userR;
        }

		public async Task<string> Register(UserW userW)
		{

            User user = await _userRepository.GetByUsername(userW.Username);
            if (user != null)
            {
                throw new ArgumentException("Username is busy");
            }

            user = UserMapper.DtoToUser(userW);
            var result = await _userRepository.Create(user);

            if (result)
            {
				
				var jwt = GenerateToken(user.Username, user.Role);
				return jwt;
			}

            throw new Exception("Error adding a user");

		}

		public void Unautorize()
        {
            throw new NotImplementedException();
        }
    }
}
