using fullstackapplication.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace fullstackapplication.Server.Data;

public static class DevelopmentDataSeeder
{
    public static async Task SeedAsync(
        ApplicationDbContext dbContext,
        IPasswordHasher<EmployeeAccount> passwordHasher,
        IConfiguration configuration,
        CancellationToken cancellationToken = default)
    {
        var demoAccounts = new[]
        {
            new { Username = "admin", DisplayName = "Demo Administrator", Role = EmployeeRoles.Admin, PasswordKey = "DemoAccounts:AdminPassword" },
            new { Username = "candidate", DisplayName = "Demo Candidate", Role = EmployeeRoles.Candidate, PasswordKey = "DemoAccounts:CandidatePassword" }
        };

        foreach (var demoAccount in demoAccounts)
        {
            var normalizedUsername = demoAccount.Username.ToUpperInvariant();
            var exists = await dbContext.EmployeeAccounts.AnyAsync(
                account => account.NormalizedUsername == normalizedUsername,
                cancellationToken);
            if (exists)
            {
                continue;
            }

            var password = configuration[demoAccount.PasswordKey];
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidOperationException($"Configure {demoAccount.PasswordKey} to seed development demo accounts.");
            }

            var account = new EmployeeAccount
            {
                Username = demoAccount.Username,
                NormalizedUsername = normalizedUsername,
                DisplayName = demoAccount.DisplayName,
                Role = demoAccount.Role,
                IsActive = true
            };
            account.PasswordHash = passwordHasher.HashPassword(account, password);
            dbContext.EmployeeAccounts.Add(account);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
