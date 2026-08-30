using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Files.Domain.Entities;

namespace Files.Infrastructure.Persistence.EntityConfigurations;

public class FileConfig : IEntityTypeConfiguration<Files.Domain.Entities.File>
{
    public void Configure(EntityTypeBuilder<Files.Domain.Entities.File> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ObjectKey)
            .IsRequired()
            .HasMaxLength(512);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Size)
            .HasColumnType("bigint");

        builder.Property(x => x.Purpose)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CompletedAt);

        builder.Property(x => x.FailureReason)
            .HasMaxLength(1000);

        builder.HasIndex(x => x.ObjectKey)
            .IsUnique();
    }
}
