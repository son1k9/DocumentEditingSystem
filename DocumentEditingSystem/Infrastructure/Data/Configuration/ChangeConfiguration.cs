using API.Domain.Core.DocumentAggregate;
using API.Domain.DocumentManagement.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class ChangeConfiguration : IEntityTypeConfiguration<Change>
	{
		public void Configure(EntityTypeBuilder<Change> builder)
		{
			builder.ToTable("Changes");

			builder.HasKey(p => p.Id);
			builder.HasOne<Document>().WithMany().HasForeignKey(p => p.DocumentId);
			builder.HasOne<User>().WithMany().HasForeignKey(p => p.EditorId);
		}
	}
}
