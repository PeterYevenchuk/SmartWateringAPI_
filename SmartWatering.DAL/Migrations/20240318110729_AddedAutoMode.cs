using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartWatering.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddedAutoMode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AutoMode",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AutoMode",
                table: "Users");
        }
    }
}
