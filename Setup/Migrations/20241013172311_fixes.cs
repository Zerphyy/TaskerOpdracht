using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class fixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpelerStats_Speler_SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SpelerStats",
                table: "SpelerStats");

            migrationBuilder.DropIndex(
                name: "IX_SpelerStats_SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "SpelerStats");

            migrationBuilder.DropColumn(
                name: "SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.AddColumn<string>(
                name: "Speler",
                table: "SpelerStats",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpelerStats",
                table: "SpelerStats",
                column: "Speler");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SpelerStats",
                table: "SpelerStats");

            migrationBuilder.DropColumn(
                name: "Speler",
                table: "SpelerStats");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "SpelerStats",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "SpelerEmail",
                table: "SpelerStats",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SpelerStats",
                table: "SpelerStats",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_SpelerStats_SpelerEmail",
                table: "SpelerStats",
                column: "SpelerEmail");

            migrationBuilder.AddForeignKey(
                name: "FK_SpelerStats_Speler_SpelerEmail",
                table: "SpelerStats",
                column: "SpelerEmail",
                principalTable: "Speler",
                principalColumn: "Email");
        }
    }
}
