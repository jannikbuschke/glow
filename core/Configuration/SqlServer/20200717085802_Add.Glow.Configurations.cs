using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Glow.Core.Configuration.SqlServer
{
    public partial class AddGlowConfigurations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlowConfigurations",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Values = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlowConfigurations", x => new { x.Id, x.Version });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlowConfigurations");
        }
    }
}