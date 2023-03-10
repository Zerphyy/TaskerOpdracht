using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class DBAlterations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Onderwerp = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Bericht = table.Column<string>(type: "nvarchar(600)", maxLength: 600, nullable: false),
                    Nieuwsbrief = table.Column<bool>(type: "bit", nullable: false),
                    Bellen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactData", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DamBord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamBord", x => x.Id);
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

            migrationBuilder.CreateTable(
                name: "Speler",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wachtwoord = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Speler", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "DamBordVakje",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Row = table.Column<int>(type: "int", nullable: false),
                    Col = table.Column<int>(type: "int", nullable: false),
                    DamStukId = table.Column<int>(type: "int", nullable: false),
                    DamBordId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamBordVakje", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamBordVakje_DamBord_DamBordId",
                        column: x => x.DamBordId,
                        principalTable: "DamBord",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DamBordVakje_DamStuk_DamStukId",
                        column: x => x.DamStukId,
                        principalTable: "DamStuk",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DamSpel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpelNaam = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WinnaarId = table.Column<int>(type: "int", nullable: true),
                    CreatorId = table.Column<int>(type: "int", nullable: false),
                    DeelnemerId = table.Column<int>(type: "int", nullable: true),
                    DamBordId = table.Column<int>(type: "int", nullable: false),
                    IsSpelVoorbij = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DamSpel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DamSpel_DamBord_DamBordId",
                        column: x => x.DamBordId,
                        principalTable: "DamBord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamSpel_Speler_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Speler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DamSpel_Speler_DeelnemerId",
                        column: x => x.DeelnemerId,
                        principalTable: "Speler",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_DamSpel_Speler_WinnaarId",
                        column: x => x.WinnaarId,
                        principalTable: "Speler",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "SpelerStats",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SpelerID = table.Column<int>(type: "int", nullable: false),
                    AantalSpellen = table.Column<int>(type: "int", nullable: false),
                    AantalGewonnen = table.Column<int>(type: "int", nullable: false),
                    AantalVerloren = table.Column<int>(type: "int", nullable: false),
                    WinLossRatio = table.Column<int>(type: "int", nullable: false),
                    LangsteWinstreak = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpelerStats", x => x.ID);
                    table.ForeignKey(
                        name: "FK_SpelerStats_Speler_SpelerID",
                        column: x => x.SpelerID,
                        principalTable: "Speler",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DamBordVakje_DamBordId",
                table: "DamBordVakje",
                column: "DamBordId");

            migrationBuilder.CreateIndex(
                name: "IX_DamBordVakje_DamStukId",
                table: "DamBordVakje",
                column: "DamStukId");

            migrationBuilder.CreateIndex(
                name: "IX_DamSpel_CreatorId",
                table: "DamSpel",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_DamSpel_DamBordId",
                table: "DamSpel",
                column: "DamBordId");

            migrationBuilder.CreateIndex(
                name: "IX_DamSpel_DeelnemerId",
                table: "DamSpel",
                column: "DeelnemerId");

            migrationBuilder.CreateIndex(
                name: "IX_DamSpel_WinnaarId",
                table: "DamSpel",
                column: "WinnaarId");

            migrationBuilder.CreateIndex(
                name: "IX_SpelerStats_SpelerID",
                table: "SpelerStats",
                column: "SpelerID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactData");

            migrationBuilder.DropTable(
                name: "DamBordVakje");

            migrationBuilder.DropTable(
                name: "DamSpel");

            migrationBuilder.DropTable(
                name: "SpelerStats");

            migrationBuilder.DropTable(
                name: "DamStuk");

            migrationBuilder.DropTable(
                name: "DamBord");

            migrationBuilder.DropTable(
                name: "Speler");
        }
    }
}
