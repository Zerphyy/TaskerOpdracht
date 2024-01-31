using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Setup.Migrations
{
    public partial class Register20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpelerStats_Speler_SpelerID",
                table: "SpelerStats");

            migrationBuilder.DropIndex(
                name: "IX_SpelerStats_SpelerID",
                table: "SpelerStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Speler",
                table: "Speler");

            migrationBuilder.DropColumn(
                name: "SpelerID",
                table: "SpelerStats");

            migrationBuilder.DropColumn(
                name: "ID",
                table: "Speler");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "DamSpel");

            migrationBuilder.DropColumn(
                name: "DeelnemerId",
                table: "DamSpel");

            migrationBuilder.DropColumn(
                name: "WinnaarId",
                table: "DamSpel");

            migrationBuilder.AddColumn<string>(
                name: "SpelerEmail",
                table: "SpelerStats",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Speler",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "DamSpel",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Deelnemer",
                table: "DamSpel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Winnaar",
                table: "DamSpel",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Speler",
                table: "Speler",
                column: "Email");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpelerStats_Speler_SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.DropIndex(
                name: "IX_SpelerStats_SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Speler",
                table: "Speler");

            migrationBuilder.DropColumn(
                name: "SpelerEmail",
                table: "SpelerStats");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "DamSpel");

            migrationBuilder.DropColumn(
                name: "Deelnemer",
                table: "DamSpel");

            migrationBuilder.DropColumn(
                name: "Winnaar",
                table: "DamSpel");

            migrationBuilder.AddColumn<int>(
                name: "SpelerID",
                table: "SpelerStats",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Speler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "ID",
                table: "Speler",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "DamSpel",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DeelnemerId",
                table: "DamSpel",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WinnaarId",
                table: "DamSpel",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Speler",
                table: "Speler",
                column: "ID");

            migrationBuilder.CreateIndex(
                name: "IX_SpelerStats_SpelerID",
                table: "SpelerStats",
                column: "SpelerID");

            migrationBuilder.AddForeignKey(
                name: "FK_SpelerStats_Speler_SpelerID",
                table: "SpelerStats",
                column: "SpelerID",
                principalTable: "Speler",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
