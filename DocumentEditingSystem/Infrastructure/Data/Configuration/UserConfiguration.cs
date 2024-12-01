using API.Domain.DocumentManagement.DocumentAggregate;
using API.Domain.ValueObjects;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class UserConfiguration: IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.ToTable("Users");

			builder.HasKey(p => p.Id);

            builder.HasMany(p => p.Documents)
				.WithMany(d => d.Editors)
				.UsingEntity(j => j.ToTable("DocumentEditors"));

            builder.OwnsOne(p => p.RefreshToken, a =>
			{
				a.Property(u => u.Value).HasColumnName("RefreshToken");
				a.Property(u => u.Value).HasColumnType("varchar");

				a.Property(u => u.TokenIsBlocked).HasColumnName("TockenStatus");
				a.Property(u => u.TokenIsBlocked).HasColumnType("bool");
			});

			builder.OwnsOne(p => p.Username, a =>
			{
				a.Property(u => u.Value).HasColumnName("Username");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
				a.HasIndex(u => u.Value).IsUnique();
			});


			builder.OwnsOne(p => p.Email, a =>
			{
				a.Property(u => u.Value).HasColumnName("Email");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
			});

			builder.OwnsOne(p => p.Name, a =>
			{
				a.Property(u => u.FirstName).HasColumnName("FirstName");
				a.Property(u => u.FirstName).HasColumnType("varchar");
				a.Property(u => u.FirstName).IsRequired();

				a.Property(u => u.LastName).HasColumnName("LastName");
				a.Property(u => u.LastName).HasColumnType("varchar");
				a.Property(u => u.LastName).IsRequired();
			});

			builder.OwnsOne(p => p.Password, a =>
			{
				a.Property(u => u.Hash).HasColumnName("PasswordHash");
				a.Property(u => u.Hash).HasColumnType("varchar");
				a.Property(u => u.Hash).IsRequired();
			});

			builder.OwnsOne(p => p.PhoneNumber, a =>
			{
				a.Property(u => u.Value).HasColumnName("PhoneNumber");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
			});
		}
	}
}
