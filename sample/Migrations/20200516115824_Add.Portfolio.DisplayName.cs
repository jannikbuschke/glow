using Microsoft.EntityFrameworkCore.Migrations;

namespace Glow.Sample.Migrations
{
    public partial class AddPortfolioDisplayName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "Portfolios",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "Portfolios");
        }
    }
}
