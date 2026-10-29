using fullstackapplication.Server.Contracts;
using fullstackapplication.Server.Data;
using fullstackapplication.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace fullstackapplication.Server.Controllers;

[ApiController]
[Route("api")]
[Authorize(Roles = EmployeeRoles.Admin)]
public sealed class EmployeeDirectoryController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet("admins")]
    [ProducesResponseType<IReadOnlyList<AccountSummaryResponse>>(StatusCodes.Status200OK)]
    public Task<List<AccountSummaryResponse>> GetAdmins(CancellationToken cancellationToken) =>
        GetAccounts(EmployeeRoles.Admin, cancellationToken);

    [HttpGet("candidates")]
    [ProducesResponseType<IReadOnlyList<AccountSummaryResponse>>(StatusCodes.Status200OK)]
    public Task<List<AccountSummaryResponse>> GetCandidates(CancellationToken cancellationToken) =>
        GetAccounts(EmployeeRoles.Candidate, cancellationToken);

    private Task<List<AccountSummaryResponse>> GetAccounts(string role, CancellationToken cancellationToken) =>
        dbContext.EmployeeAccounts
            .AsNoTracking()
            .Where(account => account.Role == role && account.IsActive)
            .OrderBy(account => account.DisplayName)
            .Select(account => new AccountSummaryResponse(
                account.Id,
                account.Username,
                account.DisplayName,
                account.Role))
            .ToListAsync(cancellationToken);
}
