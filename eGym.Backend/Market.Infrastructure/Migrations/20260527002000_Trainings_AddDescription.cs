using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20260527002000_Trainings_AddDescription")]
    public partial class Trainings_AddDescription : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns c
                    WHERE c.object_id = OBJECT_ID(N'[dbo].[Trainings]', N'U') AND c.name = N'Description')
                BEGIN
                    ALTER TABLE [dbo].[Trainings] ADD [Description] nvarchar(1000) NULL;
                END
                """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'[dbo].[Trainings]', N'U') AND c.name = N'Description')
                    ALTER TABLE [dbo].[Trainings] DROP COLUMN [Description];
                """);
        }
    }
}
