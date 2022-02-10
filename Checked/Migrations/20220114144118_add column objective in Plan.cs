using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class addcolumnobjectiveinPlan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Objective",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Objective",
                table: "Plans");
        }
    }
}
