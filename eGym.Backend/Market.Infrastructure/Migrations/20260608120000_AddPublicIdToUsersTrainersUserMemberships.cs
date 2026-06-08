using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260608120000_AddPublicIdToUsersTrainersUserMemberships")]
public partial class AddPublicIdToUsersTrainersUserMemberships : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "PublicId",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE [Users] SET PublicId = CONVERT(nvarchar(36), NEWID()) WHERE PublicId IS NULL");

        migrationBuilder.AlterColumn<string>(
            name: "PublicId",
            table: "Users",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Users_PublicId",
            table: "Users",
            column: "PublicId",
            unique: true);

        migrationBuilder.AddColumn<string>(
            name: "PublicId",
            table: "Trainers",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE [Trainers] SET PublicId = CONVERT(nvarchar(36), NEWID()) WHERE PublicId IS NULL");

        migrationBuilder.AlterColumn<string>(
            name: "PublicId",
            table: "Trainers",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Trainers_PublicId",
            table: "Trainers",
            column: "PublicId",
            unique: true);

        migrationBuilder.AddColumn<string>(
            name: "PublicId",
            table: "UserMemberships",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: true);

        migrationBuilder.Sql(
            "UPDATE [UserMemberships] SET PublicId = CONVERT(nvarchar(36), NEWID()) WHERE PublicId IS NULL");

        migrationBuilder.AlterColumn<string>(
            name: "PublicId",
            table: "UserMemberships",
            type: "nvarchar(36)",
            maxLength: 36,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "nvarchar(36)",
            oldMaxLength: 36,
            oldNullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_UserMemberships_PublicId",
            table: "UserMemberships",
            column: "PublicId",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_UserMemberships_PublicId",
            table: "UserMemberships");

        migrationBuilder.DropColumn(
            name: "PublicId",
            table: "UserMemberships");

        migrationBuilder.DropIndex(
            name: "IX_Trainers_PublicId",
            table: "Trainers");

        migrationBuilder.DropColumn(
            name: "PublicId",
            table: "Trainers");

        migrationBuilder.DropIndex(
            name: "IX_Users_PublicId",
            table: "Users");

        migrationBuilder.DropColumn(
            name: "PublicId",
            table: "Users");
    }
}
