using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class correcao : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Occurrences_occurrenceId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "occurrenceId",
                table: "Actions",
                newName: "OccurrenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_occurrenceId",
                table: "Actions",
                newName: "IX_Actions_OccurrenceId");

            migrationBuilder.AlterColumn<int>(
                name: "OccurrenceId",
                table: "Actions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Occurrences_OccurrenceId",
                table: "Actions",
                column: "OccurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actions_Occurrences_OccurrenceId",
                table: "Actions");

            migrationBuilder.RenameColumn(
                name: "OccurrenceId",
                table: "Actions",
                newName: "occurrenceId");

            migrationBuilder.RenameIndex(
                name: "IX_Actions_OccurrenceId",
                table: "Actions",
                newName: "IX_Actions_occurrenceId");

            migrationBuilder.AlterColumn<int>(
                name: "occurrenceId",
                table: "Actions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Actions_Occurrences_occurrenceId",
                table: "Actions",
                column: "occurrenceId",
                principalTable: "Occurrences",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
