#nullable disable

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Files.Infrastructure.Persistence.Migrations;

[DbContext(typeof(EfFilesDbContext))]
internal class EfFilesDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("files");

        modelBuilder.Entity("Files.Domain.Entities.File", b =>
        {
            b.Property<Guid>("Id").ValueGeneratedNever().HasColumnType("uuid");

            b.Property<string>("ObjectKey").IsRequired().HasMaxLength(512).HasColumnType("character varying(512)");

            b.Property<string>("FileName").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");

            b.Property<string>("ContentType").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");

            b.Property<long?>("Size").HasColumnType("bigint");

            b.Property<int>("Purpose").HasColumnType("integer");

            b.Property<int>("Status").HasColumnType("integer");

            b.Property<DateTime>("CreatedAt").HasColumnType("timestamp with time zone");

            b.Property<DateTime?>("CompletedAt").HasColumnType("timestamp with time zone");

            b.Property<string>("FailureReason").HasMaxLength(1000).HasColumnType("character varying(1000)");

            b.HasKey("Id");

            b.HasIndex("ObjectKey").IsUnique();

            b.ToTable("Files", "files");
        });
    }
}