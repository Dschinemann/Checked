using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class removecolumnStatusActionOccurrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StatusAction",
                table: "Occurrences");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusAction",
                table: "Occurrences",
                type: "int",
                nullable: true);
        }
    }
}
