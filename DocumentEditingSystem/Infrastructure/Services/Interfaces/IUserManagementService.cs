using API.Domain.ValueObjects;
using API.Dtos.Read;
using API.Dtos.Write;
using API.TokenConfig;
using System.IdentityModel.Tokens.Jwt;

namespace API.Infrastructure.Services.Interfaces
{
    public interface IUserManagementService
    {
        Task<Token> Register(UserW userW);
        Task<Token> Autorize(Username username, Password password);

        Task<string> GetAccessToken(Username username, string refreshToken);
        Task<bool> Unautorize(Username username);
        Task<UserR> ChangeName(Name name, Username username);
        Task<UserR> ChangePassword(Password newPassword, Password oldPassword, Username useername);
        Task<UserR> ChangeEmail(Email email, Username username);
        Task<UserR> ChangePhoneNumber(PhoneNumber phoneNumber, Username username);
        Task<UserR> GetUserData(Username username);

    }
}
