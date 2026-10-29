using fullstackapplication.Server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fullstackapplication.Server.Migrations;

[DbContext(typeof(ApplicationDbContext))]
public sealed class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "9.0.22")
            .HasAnnotation("Relational:MaxIdentifierLength", 128);

        SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

        modelBuilder.Entity("fullstackapplication.Server.Models.EmployeeAccount", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("int");

            SqlServerPropertyBuilderExtensions.UseIdentityColumn(entity.Property<int>("Id"));

            entity.Property<string>("DisplayName")
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            entity.Property<bool>("IsActive")
                .HasColumnType("bit");

            entity.Property<string>("NormalizedUsername")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            entity.Property<string>("PasswordHash")
                .IsRequired()
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            entity.Property<string>("Role")
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnType("nvarchar(20)");

            entity.Property<string>("Username")
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("nvarchar(50)");

            entity.HasKey("Id");
            entity.HasIndex("NormalizedUsername").IsUnique();
            entity.ToTable("EmployeeAccounts");
        });
#pragma warning restore 612, 618
    }
}
