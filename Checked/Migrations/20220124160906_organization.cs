using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class organization : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "organizationId",
                table: "Plans",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Plans_organizationId",
                table: "Plans",
                column: "organizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Plans_Organizations_organizationId",
                table: "Plans",
                column: "organizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Plans_Organizations_organizationId",
                table: "Plans");

            migrationBuilder.DropIndex(
                name: "IX_Plans_organizationId",
                table: "Plans");

            migrationBuilder.DropColumn(
                name: "organizationId",
                table: "Plans");
        }
    }
}
