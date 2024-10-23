using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class TestUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "kevinspijker@kpnmail.nl",
                column: "Wachtwoord",
                value: "$2b$10$kyfsDChfJWglYEwGQOra1eL2qtDvPwPPg1QWyxxBpEKNvkLcDsJy.");

            migrationBuilder.InsertData(
                table: "Speler",
                columns: new[] { "Email", "Naam", "Rol", "Wachtwoord" },
                values: new object[] { "testmail@example.com", "TestUser", "Gebruiker", "$2b$10$dbhYrxAIK0BZV7TVi/HB9e0IxhXnMudnxWgGlPuhGEuCtzYrxXtRS" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "testmail@example.com");

            migrationBuilder.UpdateData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "kevinspijker@kpnmail.nl",
                column: "Wachtwoord",
                value: "$2b$10$JwvhN2r6HGRb4ZERckXxFusoNapW0o.gU9PWnnOfLpQUbRO2zyZzS");
        }
    }
}
