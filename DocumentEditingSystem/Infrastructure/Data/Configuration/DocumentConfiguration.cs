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


			builder.OwnsOne(p => p.DocumentName, a =>
			{
				a.Property(u => u.Value).HasColumnName("DocumentName");
				a.Property(u => u.Value).HasColumnType("varchar");
				a.Property(u => u.Value).IsRequired();
			});
		}
	}
}
