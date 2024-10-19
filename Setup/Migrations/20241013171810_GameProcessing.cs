using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class GameProcessing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LangsteWinstreak",
                table: "SpelerStats");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LangsteWinstreak",
                table: "SpelerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
