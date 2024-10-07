using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class AanZet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AanZet",
                table: "DamSpel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AanZet",
                table: "DamSpel");
        }
    }
}
