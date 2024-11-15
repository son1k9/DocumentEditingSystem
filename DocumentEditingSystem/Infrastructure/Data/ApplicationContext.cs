using API.Domain.Core.DocumentAggregate;
using API.Domain.DocumentManagement.DocumentAggregate;
using API.Infrastructure.Data.Configuration;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace API.Infrastructure.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Document> Documents { get; set; }
		public DbSet<User> Users { get; set; }
		//public DbSet<DocumentManager> DocumentManagers {  get; set; }
		//public DbSet<DocumentEditor> DocumentEditors {  get; set; }
		//public DbSet<EditingDocument> EditingDocuments { get; set; }
		//public DbSet<DocumentManager> DocumentManagers {  get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DocumentConfiguration());
            //modelBuilder.ApplyConfiguration(new DocumentManagerConfiguration());

            //modelBuilder.Entity<User>().
		}

    }
}
