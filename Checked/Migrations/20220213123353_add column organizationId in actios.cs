using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class addcolumnorganizationIdinactios : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "Actions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "ed96dafb-2349-4a73-8b0c-a233aec6f877");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_OrganizationId",
                table: "Actions",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Organizations_OrganizationId",
                table: "Actions",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Organizations_OrganizationId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_OrganizationId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "Actions");
        }
    }
}
