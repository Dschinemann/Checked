using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Checked.Migrations
{
    public partial class adicionadoplanodeação : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Actions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Subject = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accountable = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Init = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Finish = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    What = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Why = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Where = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Who = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    When = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    How = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HowMuch = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TP_Status = table.Column<int>(type: "int", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    OrganizationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Actions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Actions_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Actions_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Actions_ApplicationUserId",
                table: "Actions",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Actions_OrganizationId",
                table: "Actions",
                column: "OrganizationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Actions");
        }
    }
}
