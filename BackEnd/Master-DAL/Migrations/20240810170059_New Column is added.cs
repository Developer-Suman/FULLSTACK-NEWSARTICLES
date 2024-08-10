using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Master_DAL.Migrations
{
    /// <inheritdoc />
    public partial class NewColumnisadded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Rank",
                table: "SubModules",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "SubModules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetUrl",
                table: "SubModules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "iconUrl",
                table: "SubModules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetUrl",
                table: "Modules",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "TargetUrl",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "iconUrl",
                table: "SubModules");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Modules");

            migrationBuilder.DropColumn(
                name: "TargetUrl",
                table: "Modules");
        }
    }
}
