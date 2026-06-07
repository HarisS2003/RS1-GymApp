using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260607130000_MembershipEvents_OptionalUserMembershipFk")]
public partial class MembershipEvents_OptionalUserMembershipFk : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UserMembershipId",
            table: "MembershipEvents",
            type: "int",
            nullable: true,
            oldClrType: typeof(int),
            oldType: "int");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "UserMembershipId",
            table: "MembershipEvents",
            type: "int",
            nullable: false,
            defaultValue: 0,
            oldClrType: typeof(int),
            oldType: "int",
            oldNullable: true);
    }
}
