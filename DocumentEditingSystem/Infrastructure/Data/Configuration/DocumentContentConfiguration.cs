using API.Domain.Core.DocumentAggregate;
using API.Domain.DocumentManagement.DocumentAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Infrastructure.Data.Configuration;

public class DocumentContentConfiguration : IEntityTypeConfiguration<DocumentContent>
{
    public void Configure(EntityTypeBuilder<DocumentContent> builder)
    {
        builder.ToTable("DocumentContents");

        builder.HasKey(x => x.DocumentId);
    }
}
