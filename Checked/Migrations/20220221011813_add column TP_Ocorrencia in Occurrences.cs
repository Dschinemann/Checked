using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class addcolumnTP_OcorrenciainOccurrences : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TP_OcorrenciaId",
                table: "Occurrences",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_TP_OcorrenciaId",
                table: "Occurrences",
                column: "TP_OcorrenciaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_TP_Ocorrencias_TP_OcorrenciaId",
                table: "Occurrences",
                column: "TP_OcorrenciaId",
                principalTable: "TP_Ocorrencias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_TP_Ocorrencias_TP_OcorrenciaId",
                table: "Occurrences");

            migrationBuilder.DropIndex(
                name: "IX_Occurrences_TP_OcorrenciaId",
                table: "Occurrences");

            migrationBuilder.DropColumn(
                name: "TP_OcorrenciaId",
                table: "Occurrences");
        }
    }
}
