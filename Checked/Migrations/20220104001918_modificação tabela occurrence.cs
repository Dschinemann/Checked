using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class modificaçãotabelaoccurrence : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Occurrences_AspNetUsers_OriginatorId",
                table: "Occurrences");

            migrationBuilder.DropIndex(
                name: "IX_Occurrences_OriginatorId",
                table: "Occurrences");

            migrationBuilder.DropColumn(
                name: "OriginatorId",
                table: "Occurrences");

            migrationBuilder.AddColumn<string>(
                name: "Origin",
                table: "Occurrences",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Occurrences");

            migrationBuilder.AddColumn<string>(
                name: "OriginatorId",
                table: "Occurrences",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Occurrences_OriginatorId",
                table: "Occurrences",
                column: "OriginatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Occurrences_AspNetUsers_OriginatorId",
                table: "Occurrences",
                column: "OriginatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
