using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class ajustenatabelaoccurrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_Plans_OccurrenceId",
                table: "Occurrences");

            migrationBuilder.DropIndex(
                name: "IX_Occurrences_OccurrenceId",
                table: "Occurrences");

            migrationBuilder.DropColumn(
                name: "OccurrenceId",
                table: "Occurrences");


            migrationBuilder.AlterColumn<string>(
                name: "PlanId",
                table: "Occurrences",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_PlanId",
                table: "Occurrences",
                column: "PlanId",
                unique: false,
                filter: "[PlanId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_Plans_PlanId",
                table: "Occurrences",
                column: "PlanId",
                principalTable: "Plans",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_Plans_PlanId",
                table: "Occurrences");

            migrationBuilder.DropIndex(
                name: "IX_Occurrences_PlanId",
                table: "Occurrences");


            migrationBuilder.AlterColumn<int>(
                name: "PlanId",
                table: "Occurrences",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OccurrenceId",
                table: "Occurrences",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_OccurrenceId",
                table: "Occurrences",
                column: "OccurrenceId",
                unique: true,
                filter: "[OccurrenceId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_Plans_OccurrenceId",
                table: "Occurrences",
                column: "OccurrenceId",
                principalTable: "Plans",
                principalColumn: "Id");
        }
    }
}
