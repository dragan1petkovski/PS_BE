using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace be.Migrations
{
    /// <inheritdoc />
    public partial class t2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientDBDMTeamDBDM");

            migrationBuilder.AddColumn<Guid>(
                name: "clientid",
                table: "Teams",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Teams_clientid",
                table: "Teams",
                column: "clientid");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_Clients_clientid",
                table: "Teams",
                column: "clientid",
                principalTable: "Clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_Clients_clientid",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_clientid",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "clientid",
                table: "Teams");

            migrationBuilder.CreateTable(
                name: "ClientDBDMTeamDBDM",
                columns: table => new
                {
                    clientsid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    teamsid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDBDMTeamDBDM", x => new { x.clientsid, x.teamsid });
                    table.ForeignKey(
                        name: "FK_ClientDBDMTeamDBDM_Clients_clientsid",
                        column: x => x.clientsid,
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientDBDMTeamDBDM_Teams_teamsid",
                        column: x => x.teamsid,
                        principalTable: "Teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientDBDMTeamDBDM_teamsid",
                table: "ClientDBDMTeamDBDM",
                column: "teamsid");
        }
    }
}
