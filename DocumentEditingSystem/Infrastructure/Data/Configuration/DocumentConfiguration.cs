using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class DocumentConfiguration : IEntityTypeConfiguration<Document>
	{
		public void Configure(EntityTypeBuilder<Document> builder)
		{
			builder.ToTable("Documents");

			builder.HasKey(e => e.Id);

			builder.HasOne<User>().WithMany().HasForeignKey(p => p.OwnerId);

            builder.HasMany(p => p.Editors)
				.WithMany(u => u.Documents)
				.UsingEntity(j => j.ToTable("DocumentEditors"));

            builder.OwnsOne(p => p.DocumentName, a =>
			{
				a.Property(u => u.Value).HasColumnName("DocumentName");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
			});

			builder.HasOne(d => d.Content).WithOne(c => c.Document)
				.HasForeignKey<DocumentContent>(c => c.DocumentId).IsRequired();
		}
	}
}
