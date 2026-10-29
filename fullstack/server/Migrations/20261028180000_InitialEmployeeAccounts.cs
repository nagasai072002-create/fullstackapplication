using fullstackapplication.Server.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace fullstackapplication.Server.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261028180000_InitialEmployeeAccounts")]
public sealed class InitialEmployeeAccounts : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "EmployeeAccounts",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                Username = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                NormalizedUsername = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                DisplayName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                IsActive = table.Column<bool>(type: "bit", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_EmployeeAccounts", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_EmployeeAccounts_NormalizedUsername",
            table: "EmployeeAccounts",
            column: "NormalizedUsername",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "EmployeeAccounts");
    }
}
