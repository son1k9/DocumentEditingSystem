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
        //public DbSet<Document> Documents { get; set; }
		public DbSet<User> Users { get; set; }
		//public DbSet<DocumentManager> DocumentUsers {  get; set; }
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

   //         modelBuilder.Entity<User>().HasKey(p => p.Id);
   //         modelBuilder.Entity<User>().OwnsOne(p => p.Name, a =>
   //         {
   //             a.Property(u => u.Value).HasColumnName("Name").IsRequired();
   //             a.Property(u => u.Value).HasColumnType("varchar");
   //         });
			//modelBuilder.Entity<User>().OwnsOne(p => p.Email, a =>
   //         {
   //             a.Property(u => u.Value).HasColumnName("Email").IsRequired();
   //         });
			//modelBuilder.Entity<User>().OwnsOne(p => p.Username, a =>
   //         {
   //             a.Property(u => u.Value).HasColumnName("Username")
   //             .HasColumnType("varchar")
   //             .IsRequired();
   //         });
			//modelBuilder.Entity<User>().OwnsOne(p => p.Password, a =>
   //         {
   //             a.Property(u => u.Hash).HasColumnName("PasswordHash");
   //             a.Property(u => u.Hash).HasColumnType("varchar");
   //             a.Property(u => u.Hash).IsRequired();
			//});
			//modelBuilder.Entity<User>().OwnsOne(p => p.PhoneNumber, a =>
   //         {
   //             a.Property(u => u.Value).HasColumnName("PhoneNumber").IsRequired();
   //         });
			//modelBuilder.Entity<User>().Property()
			//modelBuilder.Entity<DocumentManager>().HasOne<User>();
		}

    }
}
