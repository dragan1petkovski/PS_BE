using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerDB.Migrations
{
    /// <inheritdoc />
    public partial class t2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "deleteVerifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    requestorId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    type = table.Column<int>(type: "int", nullable: false),
                    itemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    verificationCode = table.Column<int>(type: "int", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deleteVerifications", x => x.id);
                    table.ForeignKey(
                        name: "FK_deleteVerifications_AspNetUsers_requestorId",
                        column: x => x.requestorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_deleteVerifications_requestorId",
                table: "deleteVerifications",
                column: "requestorId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "deleteVerifications");
        }
    }
}
