using fullstackapplication.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace fullstackapplication.Server.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<EmployeeAccount> EmployeeAccounts => Set<EmployeeAccount>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var account = modelBuilder.Entity<EmployeeAccount>();
        account.HasKey(employee => employee.Id);
        account.HasIndex(employee => employee.NormalizedUsername).IsUnique();
        account.Property(employee => employee.Username).HasMaxLength(50).IsRequired();
        account.Property(employee => employee.NormalizedUsername).HasMaxLength(50).IsRequired();
        account.Property(employee => employee.DisplayName).HasMaxLength(100).IsRequired();
        account.Property(employee => employee.Role).HasMaxLength(20).IsRequired();
        account.Property(employee => employee.PasswordHash).HasMaxLength(500).IsRequired();
        account.Property(employee => employee.IsActive).IsRequired();
    }
}
