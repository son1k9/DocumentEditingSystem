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
using System.Security.Cryptography;
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

        private string GenerateAccessToken(Username username, int userId)
        {
			var claims = new List<Claim> {
                new Claim(ClaimsIdentity.DefaultNameClaimType, username.Value ),
				new Claim(ClaimTypes.NameIdentifier, userId.ToString())
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
				expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
				signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return encodedJwt;
		}

        private string GenerateRefreshToken()
        {
            var refreshToken = new byte[64];
            var rng = RandomNumberGenerator.Create();
            rng.GetBytes(refreshToken);
            return Convert.ToBase64String(refreshToken);
        }

        public async Task<Token> Autorize(Username username, Password password)
        {
            User user = await _userRepository.GetByUsernameAsync(username.Value);
			
            if(user == null)
            {
                throw new ArgumentException("Invalid login or password!");
            }

            var result = user.Password.Compare(password);

            if (!result)
            {
                throw new ArgumentException("Invalid login or password!");
            }

            var jwt = GenerateAccessToken(username, user.Id);
            var refreshToken = GenerateRefreshToken();
            user.SetRefreshToken(new RefreshToken(refreshToken));
            result = await _userRepository.UpdateAsync(user);

            if (!result)
            {
                throw new Exception("Failed to install refresh token");
            }

            return new Token
            {
                AccessToken = jwt,
                RefreshToken = refreshToken,
            };
		}

        public async Task<UserR> ChangeName(Name name, Username username)
        {
            User user = await _userRepository.GetByUsernameAsync(username.Value);

            if(user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            user.ChangeName(name);

            bool result = await _userRepository.UpdateAsync(user);

            if (!result)
            {
				throw new Exception("An error occurred updating the data");
			}

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> ChangeEmail(Email email, Username username)
        {
            User user = await _userRepository.GetByUsernameAsync(username.Value);

            if(user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            user.ChangeEmail(email);
            bool result = await _userRepository.UpdateAsync(user);
            if (!result)
            {
				throw new Exception("An error occurred updating the data");
			}

            return UserMapper.UserToDto(user);
        }

        public async Task<UserR> ChangePassword(Password newPassword, Password oldPassword, Username username)
        {
			User user = await _userRepository.GetByUsernameAsync(username.Value);

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
			bool result = await _userRepository.UpdateAsync(user);

			if (!result)
			{
				throw new Exception("An error occurred updating the data");
			}

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> ChangePhoneNumber(PhoneNumber newNumber, Username username)
        {
			User user = await _userRepository.GetByUsernameAsync(username.Value);

			if (user == null)
			{
				throw new ArgumentException("The user does not exist");
			}

			user.ChangePhoneNumber(newNumber);
			bool result = await _userRepository.UpdateAsync(user);

			if (!result) throw new Exception("An error occurred updating the data");

			return UserMapper.UserToDto(user);
		}

        public async Task<UserR> GetUserData(Username username)
        {
            User user = await _userRepository.GetByUsernameAsync(username.Value);
            if (user == null)
            {
                throw new ArgumentException("The user does not exist");
            }

            UserR userR = UserMapper.UserToDto(user);
            return userR;
        }

		public async Task<Token> Register(UserW userW)
		{

            User user = await _userRepository.GetByUsernameAsync(userW.Username);
            if (user != null)
            {
                throw new ArgumentException("Username is busy");
            }

            user = UserMapper.DtoToUser(userW);
            var result = await _userRepository.CreateAsync(user);

            if (result)
            {
				
				var jwt = GenerateAccessToken(user.Username, user.Id);
                var refreshToken = GenerateRefreshToken();
                user.SetRefreshToken(new RefreshToken(refreshToken));
                result = await _userRepository.UpdateAsync(user); 

                if (!result) throw new Exception("Failed to install refresh token");

				return new Token
                {
                    AccessToken = jwt,
                    RefreshToken = refreshToken
                };
			}

            throw new Exception("Error adding a user");

		}

		public async Task<bool> Unautorize(Username username)
        {
            User user = await _userRepository.GetByUsernameAsync(username.Value);
            if (user == null) return false;

            user.BlockRefreshToken();
            var result = await _userRepository.UpdateAsync(user);
            if (!result) return false;

            return true;
        }

		public async Task<string> GetAccessToken(Username username, string refreshToken)
		{
			User user = await _userRepository.GetByUsernameAsync(username.Value);

            if (user == null)
            {
                throw new ArgumentException("User not found!");
            }

            var result = user.RefreshToken.ValidateToken(new RefreshToken(refreshToken));

            if(!result)
            {
                throw new ArgumentException("Token is invalid!");
            }

            var jwt = GenerateAccessToken(user.Username, user.Id);

            return jwt;

		}
	}
}
