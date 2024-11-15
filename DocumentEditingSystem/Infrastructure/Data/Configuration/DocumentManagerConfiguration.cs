using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class DocumentManagerConfiguration : IEntityTypeConfiguration<DocumentManager>
	{
		public void Configure(EntityTypeBuilder<DocumentManager> builder)
		{
			builder.ToTable("DocumentManagers");
			builder.HasKey(p => p.Id);

			builder.OwnsOne(p => p.Username, a =>
			{
				a.Property(u => u.Value).HasColumnName("Username");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
				a.HasIndex(u => u.Value).IsUnique();
			});
		}
	}
}
