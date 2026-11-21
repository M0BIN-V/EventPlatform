using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Files.Infrastructure.Persistence.EntityConfigurations;

public class FileConfig : IEntityTypeConfiguration<File>
{
    public void Configure(EntityTypeBuilder<File> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ObjectKey)
            .HasMaxLength(500);

        builder.Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.ContentType)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Owner)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Purpose)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(x => x.Size)
            .HasColumnType("bigint");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}