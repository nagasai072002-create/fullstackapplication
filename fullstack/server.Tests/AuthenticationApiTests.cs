using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using fullstackapplication.Server.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace fullstackapplication.Server.Tests;

public sealed class AuthenticationApiTests : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client;

    public AuthenticationApiTests(ApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginIssuesRoleTokenAndOnlyAdminsCanReadAccountLists()
    {
        var swaggerUi = await _client.GetAsync("/swagger/index.html");
        swaggerUi.EnsureSuccessStatusCode();
        var swaggerDocument = await _client.GetStringAsync("/swagger/v1/swagger.json");
        Assert.Contains("Bearer", swaggerDocument, StringComparison.OrdinalIgnoreCase);

        var candidateLogin = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "candidate",
            password = "CandidateDemo123!"
        });
        candidateLogin.EnsureSuccessStatusCode();
        using var candidateJson = JsonDocument.Parse(await candidateLogin.Content.ReadAsStringAsync());
        var candidateToken = candidateJson.RootElement.GetProperty("accessToken").GetString();
        Assert.False(string.IsNullOrWhiteSpace(candidateToken));
        Assert.False(candidateJson.RootElement.GetProperty("user").TryGetProperty("passwordHash", out _));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", candidateToken);
        var deniedList = await _client.GetAsync("/api/admins");
        Assert.Equal(HttpStatusCode.Forbidden, deniedList.StatusCode);

        var adminLogin = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "admin",
            password = "AdminDemo123!"
        });
        adminLogin.EnsureSuccessStatusCode();
        using var adminJson = JsonDocument.Parse(await adminLogin.Content.ReadAsStringAsync());
        var adminToken = adminJson.RootElement.GetProperty("accessToken").GetString();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

        var adminsResponse = await _client.GetAsync("/api/admins");
        adminsResponse.EnsureSuccessStatusCode();
        var adminsBody = await adminsResponse.Content.ReadAsStringAsync();
        Assert.Contains("admin", adminsBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("passwordHash", adminsBody, StringComparison.OrdinalIgnoreCase);

        var candidatesResponse = await _client.GetAsync("/api/candidates");
        candidatesResponse.EnsureSuccessStatusCode();
        var candidatesBody = await candidatesResponse.Content.ReadAsStringAsync();
        Assert.Contains("candidate", candidatesBody, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("passwordHash", candidatesBody, StringComparison.OrdinalIgnoreCase);
    }
}

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"AuthenticationApiTests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment(Environments.Development);
        builder.ConfigureAppConfiguration(configuration => configuration.AddInMemoryCollection(
            new Dictionary<string, string?>
            {
                ["Jwt:SigningKey"] = "test-signing-key-that-is-at-least-32-bytes-long",
                ["DemoAccounts:AdminPassword"] = "AdminDemo123!",
                ["DemoAccounts:CandidatePassword"] = "CandidateDemo123!"
            }));
        builder.ConfigureServices(services =>
        {
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<ApplicationDbContext>>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));
        });
    }
}
