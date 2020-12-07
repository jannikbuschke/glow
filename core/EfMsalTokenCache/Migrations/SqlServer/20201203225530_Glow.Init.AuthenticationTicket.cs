using Microsoft.EntityFrameworkCore.Migrations;

namespace Glow.Core.EfMsalTokenStore.Migrations.SqlServer
{
    public partial class GlowInitAuthenticationTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "__MsalToken",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Value = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___MsalToken", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__MsalToken");
        }
    }
}
