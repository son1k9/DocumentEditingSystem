using API.Domain.Core.DocumentAggregate;
using Domain.UserManagement.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration
{
	public class ChangeConfiguration : IEntityTypeConfiguration<Change>
	{
		public void Configure(EntityTypeBuilder<Change> builder)
		{
			builder.HasKey(p => p.Id);
		}
	}
}
