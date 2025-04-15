using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DotnetUserManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddApplicationIdToAspNetRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApplicationId",
                table: "AspNetRoles",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApplicationId",
                table: "AspNetRoles");
        }
    }
}
