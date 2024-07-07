using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace be.Migrations
{
    /// <inheritdoc />
    public partial class t1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Passwords",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Passwords", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    firstname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    lastname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    clientid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.id);
                    table.ForeignKey(
                        name: "FK_Teams_Clients_clientid",
                        column: x => x.clientid,
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientDBDMUserDBDM",
                columns: table => new
                {
                    clientsid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usersid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDBDMUserDBDM", x => new { x.clientsid, x.usersid });
                    table.ForeignKey(
                        name: "FK_ClientDBDMUserDBDM_Clients_clientsid",
                        column: x => x.clientsid,
                        principalTable: "Clients",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientDBDMUserDBDM_Users_usersid",
                        column: x => x.usersid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PersonalFolders",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonalFolders", x => x.id);
                    table.ForeignKey(
                        name: "FK_PersonalFolders_Users_UserDBDMid",
                        column: x => x.UserDBDMid,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "TeamDBDMUserDBDM",
                columns: table => new
                {
                    teamsid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    usersid = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDBDMUserDBDM", x => new { x.teamsid, x.usersid });
                    table.ForeignKey(
                        name: "FK_TeamDBDMUserDBDM_Teams_teamsid",
                        column: x => x.teamsid,
                        principalTable: "Teams",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamDBDMUserDBDM_Users_usersid",
                        column: x => x.usersid,
                        principalTable: "Users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    friendlyname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    issuedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    issuedTo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    expirationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    passwordid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalFolderDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.id);
                    table.ForeignKey(
                        name: "FK_Certificates_Passwords_passwordid",
                        column: x => x.passwordid,
                        principalTable: "Passwords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Certificates_PersonalFolders_PersonalFolderDBDMid",
                        column: x => x.PersonalFolderDBDMid,
                        principalTable: "PersonalFolders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Certificates_Teams_TeamDBDMid",
                        column: x => x.TeamDBDMid,
                        principalTable: "Teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Certificates_Users_UserDBDMid",
                        column: x => x.UserDBDMid,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Credentials",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    domain = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    remote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    note = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    passwordid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PersonalFolderDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TeamDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UserDBDMid = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credentials", x => x.id);
                    table.ForeignKey(
                        name: "FK_Credentials_Passwords_passwordid",
                        column: x => x.passwordid,
                        principalTable: "Passwords",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Credentials_PersonalFolders_PersonalFolderDBDMid",
                        column: x => x.PersonalFolderDBDMid,
                        principalTable: "PersonalFolders",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Credentials_Teams_TeamDBDMid",
                        column: x => x.TeamDBDMid,
                        principalTable: "Teams",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_Credentials_Users_UserDBDMid",
                        column: x => x.UserDBDMid,
                        principalTable: "Users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_passwordid",
                table: "Certificates",
                column: "passwordid");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_PersonalFolderDBDMid",
                table: "Certificates",
                column: "PersonalFolderDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_TeamDBDMid",
                table: "Certificates",
                column: "TeamDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_UserDBDMid",
                table: "Certificates",
                column: "UserDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDBDMUserDBDM_usersid",
                table: "ClientDBDMUserDBDM",
                column: "usersid");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_passwordid",
                table: "Credentials",
                column: "passwordid");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_PersonalFolderDBDMid",
                table: "Credentials",
                column: "PersonalFolderDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_TeamDBDMid",
                table: "Credentials",
                column: "TeamDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_Credentials_UserDBDMid",
                table: "Credentials",
                column: "UserDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_PersonalFolders_UserDBDMid",
                table: "PersonalFolders",
                column: "UserDBDMid");

            migrationBuilder.CreateIndex(
                name: "IX_TeamDBDMUserDBDM_usersid",
                table: "TeamDBDMUserDBDM",
                column: "usersid");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_clientid",
                table: "Teams",
                column: "clientid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropTable(
                name: "ClientDBDMUserDBDM");

            migrationBuilder.DropTable(
                name: "Credentials");

            migrationBuilder.DropTable(
                name: "TeamDBDMUserDBDM");

            migrationBuilder.DropTable(
                name: "Passwords");

            migrationBuilder.DropTable(
                name: "PersonalFolders");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
