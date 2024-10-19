using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class Moderation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DamBordVakje");

            migrationBuilder.DropTable(
                name: "DamStuk");

            migrationBuilder.InsertData(
                table: "Speler",
                columns: new[] { "Email", "Naam", "Rol", "Wachtwoord" },
                values: new object[] { "kevinspijker@kpnmail.nl", "Zerphy", "Moderator", "$2b$10$fkeQYGRyK09BeRQ6EQzfA.nkFioSRwXXhyENeotAuvXW3j8ttS5q6" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Speler",
                keyColumn: "Email",
                keyValue: "kevinspijker@kpnmail.nl");

            migrationBuilder.CreateTable(
                name: "DamBordVakje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Col = table.Column<int>(type: "int", nullable: false),
                    DamBordId = table.Column<int>(type: "int", nullable: false),
                    DamStukId = table.Column<int>(type: "int", nullable: true),
                    Row = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamBordVakje", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DamStuk",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Kleur = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamStuk", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "DamStuk",
                columns: new[] { "Id", "Kleur", "Type" },
                values: new object[,]
                {
                    { 1, 0, 0 },
                    { 2, 1, 0 },
                    { 3, 0, 1 },
                    { 4, 1, 1 }
                });
        }
    }
}
