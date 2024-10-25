using API.Domain.ValueObjects;
using API.Infrastructure.Services.Interfaces;

namespace API.Infrastructure.Services.Implementations
{
    internal class UserManagementService : IUserManagementService
    {
        public void Autorize(Email email, string password)
        {

        }

        public void ChangeName()
        {
            throw new NotImplementedException();
        }

        public void Unautorize()
        {
            throw new NotImplementedException();
        }

        void IUserManagementService.Register(Name name, Email email, PhoneNumber phoneNumber, string password)
        {

        }
    }
}
