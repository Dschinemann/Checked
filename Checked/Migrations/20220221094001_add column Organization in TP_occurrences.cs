using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class addcolumnOrganizationinTP_occurrences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrganizationId",
                table: "TP_Ocorrencias",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "15f02084-e3bb-4ff8-aa53-1e34e055d341");

            migrationBuilder.CreateIndex(
                name: "IX_TP_Ocorrencias_OrganizationId",
                table: "TP_Ocorrencias",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TP_Ocorrencias_Organizations_OrganizationId",
                table: "TP_Ocorrencias",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TP_Ocorrencias_Organizations_OrganizationId",
                table: "TP_Ocorrencias");

            migrationBuilder.DropIndex(
                name: "IX_TP_Ocorrencias_OrganizationId",
                table: "TP_Ocorrencias");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "TP_Ocorrencias");
        }
    }
}
