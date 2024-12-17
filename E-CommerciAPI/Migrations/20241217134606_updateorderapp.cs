using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_CommerciAPI.Migrations
{
    /// <inheritdoc />
    public partial class updateorderapp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Orders");
            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Orders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
            migrationBuilder.AddColumn<int>(
               name: "AppUserId",
               table: "Orders",
               type: "int",
               nullable: false,
               defaultValue: 0);
        }
    }
}
