using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260607000000_Gyms_RemoveTenantBrandingFields")]
public partial class Gyms_RemoveTenantBrandingFields : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF EXISTS (
                SELECT 1
                FROM sys.indexes
                WHERE name = 'IX_Gyms_CustomDomain'
                  AND object_id = OBJECT_ID(N'[Gyms]')
            )
                DROP INDEX [IX_Gyms_CustomDomain] ON [Gyms];

            IF COL_LENGTH('Gyms', 'CustomDomain') IS NOT NULL
                ALTER TABLE [Gyms] DROP COLUMN [CustomDomain];

            IF COL_LENGTH('Gyms', 'LogoUrl') IS NOT NULL
                ALTER TABLE [Gyms] DROP COLUMN [LogoUrl];

            IF COL_LENGTH('Gyms', 'PrimaryColor') IS NOT NULL
                ALTER TABLE [Gyms] DROP COLUMN [PrimaryColor];
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CustomDomain",
            table: "Gyms",
            type: "nvarchar(255)",
            maxLength: 255,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "LogoUrl",
            table: "Gyms",
            type: "nvarchar(500)",
            maxLength: 500,
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PrimaryColor",
            table: "Gyms",
            type: "nvarchar(20)",
            maxLength: 20,
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Gyms_CustomDomain",
            table: "Gyms",
            column: "CustomDomain",
            unique: true,
            filter: "[CustomDomain] IS NOT NULL");
    }
}
