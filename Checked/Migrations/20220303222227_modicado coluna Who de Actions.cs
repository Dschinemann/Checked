using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class modicadocolunaWhodeActions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Who",
                table: "Actions");

            migrationBuilder.AddColumn<string>(
                name: "WhoId",
                table: "Actions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "3e03c45b-b2a7-4543-b728-bfef67f48516");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_WhoId",
                table: "Actions",
                column: "WhoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_AspNetUsers_WhoId",
                table: "Actions",
                column: "WhoId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_AspNetUsers_WhoId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_WhoId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "WhoId",
                table: "Actions");

            migrationBuilder.AddColumn<string>(
                name: "Who",
                table: "Actions",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
