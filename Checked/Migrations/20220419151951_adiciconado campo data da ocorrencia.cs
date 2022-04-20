using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class adiciconadocampodatadaocorrencia : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_helpDesks_AspNetUsers_CreatedById",
                table: "helpDesks");

            migrationBuilder.DropForeignKey(
                name: "FK_helpDesks_AspNetUsers_UserId",
                table: "helpDesks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_helpDesks",
                table: "helpDesks");

            migrationBuilder.RenameTable(
                name: "helpDesks",
                newName: "HelpDesks");

            migrationBuilder.RenameIndex(
                name: "IX_helpDesks_UserId",
                table: "HelpDesks",
                newName: "IX_HelpDesks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_helpDesks_CreatedById",
                table: "HelpDesks",
                newName: "IX_HelpDesks_CreatedById");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateOccurrence",
                table: "Occurrences",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_HelpDesks",
                table: "HelpDesks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HelpDesks_AspNetUsers_CreatedById",
                table: "HelpDesks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_HelpDesks_AspNetUsers_UserId",
                table: "HelpDesks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HelpDesks_AspNetUsers_CreatedById",
                table: "HelpDesks");

            migrationBuilder.DropForeignKey(
                name: "FK_HelpDesks_AspNetUsers_UserId",
                table: "HelpDesks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_HelpDesks",
                table: "HelpDesks");

            migrationBuilder.DropColumn(
                name: "DateOccurrence",
                table: "Occurrences");

            migrationBuilder.RenameTable(
                name: "HelpDesks",
                newName: "helpDesks");

            migrationBuilder.RenameIndex(
                name: "IX_HelpDesks_UserId",
                table: "helpDesks",
                newName: "IX_helpDesks_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_HelpDesks_CreatedById",
                table: "helpDesks",
                newName: "IX_helpDesks_CreatedById");

            migrationBuilder.AddPrimaryKey(
                name: "PK_helpDesks",
                table: "helpDesks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_helpDesks_AspNetUsers_CreatedById",
                table: "helpDesks",
                column: "CreatedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_helpDesks_AspNetUsers_UserId",
                table: "helpDesks",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
