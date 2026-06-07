using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260607120000_MembershipEvents_AddEventStore")]
public partial class MembershipEvents_AddEventStore : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "MembershipEvents",
            columns: table => new
            {
                Id = table.Column<int>(type: "int", nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"),
                UserMembershipId = table.Column<int>(type: "int", nullable: true),
                EventType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                EventData = table.Column<string>(type: "nvarchar(max)", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_MembershipEvents", x => x.Id);
                table.ForeignKey(
                    name: "FK_MembershipEvents_UserMemberships_UserMembershipId",
                    column: x => x.UserMembershipId,
                    principalTable: "UserMemberships",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_MembershipEvents_UserMembershipId",
            table: "MembershipEvents",
            column: "UserMembershipId");

        migrationBuilder.CreateIndex(
            name: "IX_MembershipEvents_UserMembershipId_CreatedAt",
            table: "MembershipEvents",
            columns: new[] { "UserMembershipId", "CreatedAt" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "MembershipEvents");
    }
}
