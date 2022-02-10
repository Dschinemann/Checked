using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class Addcolumnsoccurrences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectiveAction",
                table: "Occurrences",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Occurrences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StatusAction",
                table: "Occurrences",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectiveAction",
                table: "Occurrences");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Occurrences");

            migrationBuilder.DropColumn(
                name: "StatusAction",
                table: "Occurrences");
        }
    }
}
