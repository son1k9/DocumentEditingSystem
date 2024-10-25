using API.Domain.ValueObjects;
using API.Domain.ValueObjects.Enums;

namespace API.Domain.DocumentManagement.DocumentAggregate
{
    internal class DocumentManager
    {
        int Id { get; }

        Email Email { get; }

        public DocumentManager(int userId, Email email, Role role) {
            if (role != Role.Manager) throw new ArgumentException("User is not a manager");
			if (email == null) throw new ArgumentException("Email can not be null");
			Id = userId; 
            Email = email;
        }

    }
}
