using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerDB.Migrations
{
    /// <inheritdoc />
    public partial class t3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isClicked",
                table: "deleteVerifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isClicked",
                table: "deleteVerifications");
        }
    }
}
