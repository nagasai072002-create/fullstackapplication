# Employee authentication API

## Endpoints

In Development, run the API and open `/swagger` (for example, `https://localhost:7216/swagger`) to explore and call the endpoints interactively. Use `POST /api/auth/login` with the demo credentials, copy `accessToken`, select **Authorize**, and enter `Bearer <accessToken>` to call the protected list endpoints. Swagger UI and its generated Swagger document are not enabled outside Development. The existing OpenAPI JSON endpoint remains at `/openapi/v1.json`.

- `POST /api/auth/login` accepts `{"username":"admin","password":"AdminDemo123!"}` and returns a bearer token plus a safe user summary.
- `GET /api/admins` returns active Admin account summaries.
- `GET /api/candidates` returns active Candidate account summaries.

The list endpoints require an Admin JWT. Candidate accounts can authenticate, but receive `403 Forbidden` when requesting either list. Password hashes are never included in API responses.

## Development database and sample accounts

Development uses SQL Server LocalDB (`(localdb)\\mssqllocaldb`) and applies EF Core migrations when the API starts. The Development environment seeds these demo accounts if they do not already exist:

| Username | Password | Role |
| --- | --- | --- |
| `admin` | `AdminDemo123!` | Admin |
| `candidate` | `CandidateDemo123!` | Candidate |

These credentials are for local development only. Do not use them in deployed environments. The demo seeder runs only in Development, and stores passwords using ASP.NET Core's password hasher.

## Configuration

Set `ConnectionStrings__DefaultConnection` to the SQL Server connection string for the target environment. Set `Jwt__SigningKey` to a random secret of at least 32 bytes (for example via .NET User Secrets locally or a deployment secret store); optionally configure `Jwt__Issuer`, `Jwt__Audience`, and `Jwt__LifetimeMinutes`. Production startup fails when the signing key is absent or too short. Development generates a temporary, process-local key when one is not configured, so development tokens stop validating after an API restart.

EF Core code-first migrations live under `Migrations`. Add future migrations from the server project with `dotnet ef migrations add <MigrationName>` and apply them using `dotnet ef database update`; application startup also applies committed migrations.
