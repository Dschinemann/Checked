using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Accountable",
                table: "Plans");

            migrationBuilder.AddColumn<string>(
                name: "AccountableId",
                table: "Plans",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Plans_AccountableId",
                table: "Plans",
                column: "AccountableId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_AspNetUsers_AccountableId",
                table: "Plans",
                column: "AccountableId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_AspNetUsers_AccountableId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_AccountableId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "AccountableId",
                table: "Plans");

            migrationBuilder.AddColumn<string>(
                name: "Accountable",
                table: "Plans",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
