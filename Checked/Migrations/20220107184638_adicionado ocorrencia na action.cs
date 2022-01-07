using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class adicionadoocorrencianaaction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "occurrenceId",
                table: "Actions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Actions_occurrenceId",
                table: "Actions",
                column: "occurrenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Occurrences_occurrenceId",
                table: "Actions",
                column: "occurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Occurrences_occurrenceId",
                table: "Actions");

            migrationBuilder.DropIndex(
                name: "IX_Actions_occurrenceId",
                table: "Actions");

            migrationBuilder.DropColumn(
                name: "occurrenceId",
                table: "Actions");
        }
    }
}
