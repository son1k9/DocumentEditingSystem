using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class ApplicationContext : DbContext
    {
        DbSet<Document> Documents;
        DbSet<User> Users;
        DbSet<DocumentManager> DocumentUsers;
        DbSet<>
    }
}
