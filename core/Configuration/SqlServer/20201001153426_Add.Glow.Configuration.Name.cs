using Microsoft.EntityFrameworkCore.Migrations;

namespace Glow.Core.Configuration.SqlServer
{
    public partial class AddGlowConfigurationName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GlowConfigurations",
                table: "GlowConfigurations");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "GlowConfigurations",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlowConfigurations",
                table: "GlowConfigurations",
                columns: new[] { "Id", "Version", "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_GlowConfigurations",
                table: "GlowConfigurations");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "GlowConfigurations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GlowConfigurations",
                table: "GlowConfigurations",
                columns: new[] { "Id", "Version" });
        }
    }
}