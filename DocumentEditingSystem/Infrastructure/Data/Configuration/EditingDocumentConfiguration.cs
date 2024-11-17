using API.Domain.Core.DocumentAggregate;
using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class EditingDocumentConfiguration : IEntityTypeConfiguration<EditingDocument>
	{
		public void Configure(EntityTypeBuilder<EditingDocument> builder)
		{
			builder.HasKey(p => p.Id);
			builder.HasOne<User>().WithMany().HasForeignKey(p => p.OwnerId);
			//builder.HasMany<User>().WithMany();
			builder.HasOne<Document>().WithOne().HasForeignKey<EditingDocument>(p => p.Id);
		}
	}
}
