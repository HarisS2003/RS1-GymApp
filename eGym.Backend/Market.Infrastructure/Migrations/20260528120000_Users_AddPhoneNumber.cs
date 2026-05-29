using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260528120000_Users_AddPhoneNumber")]
public partial class Users_AddPhoneNumber : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PhoneNumber",
            table: "Users",
            type: "nvarchar(512)",
            maxLength: 512,
            nullable: false,
            defaultValue: "");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "PhoneNumber",
            table: "Users");
    }
}
