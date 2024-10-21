using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class Admin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "kevinspijker@kpnmail.nl",
                columns: new[] { "Rol", "Wachtwoord" },
                values: new object[] { "Admin", "$2b$10$JwvhN2r6HGRb4ZERckXxFusoNapW0o.gU9PWnnOfLpQUbRO2zyZzS" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "kevinspijker@kpnmail.nl",
                columns: new[] { "Rol", "Wachtwoord" },
                values: new object[] { "Moderator", "$2b$10$fkeQYGRyK09BeRQ6EQzfA.nkFioSRwXXhyENeotAuvXW3j8ttS5q6" });
        }
    }
}
