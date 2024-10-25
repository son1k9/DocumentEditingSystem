using API.Domain.ValueObjects;

namespace API.Infrastructure.Services.Interfaces
{
    internal interface IUserManagementService
    {
        void Register(Name name, Email email, PhoneNumber phoneNumber, string password);
        void Autorize(Email email, string password);
        void Unautorize();
        void ChangeName();
    }
}
