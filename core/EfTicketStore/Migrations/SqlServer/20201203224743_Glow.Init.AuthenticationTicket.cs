using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Glow.Core.EfTicketStore.Migrations.SqlServer
{
    public partial class GlowInitAuthenticationTicket : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "__AuthenticationTicket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    Value = table.Column<byte[]>(nullable: true),
                    LastActivity = table.Column<DateTimeOffset>(nullable: true),
                    Expires = table.Column<DateTimeOffset>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___AuthenticationTicket", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__AuthenticationTicket");
        }
    }
}
