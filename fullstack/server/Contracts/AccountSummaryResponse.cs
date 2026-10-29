namespace fullstackapplication.Server.Contracts;

public sealed record AccountSummaryResponse(int Id, string Username, string DisplayName, string Role);
