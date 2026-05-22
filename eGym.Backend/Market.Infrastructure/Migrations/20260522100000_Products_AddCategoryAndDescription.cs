using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Products_AddCategoryAndDescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns c
                    WHERE c.object_id = OBJECT_ID(N'[dbo].[Products]', N'U') AND c.name = N'CategoryName')
                BEGIN
                    ALTER TABLE [dbo].[Products] ADD [CategoryName] nvarchar(100) NOT NULL
                        CONSTRAINT DF_Products_CategoryName DEFAULT N'';
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns c
                    WHERE c.object_id = OBJECT_ID(N'[dbo].[Products]', N'U') AND c.name = N'Description')
                BEGIN
                    ALTER TABLE [dbo].[Products] ADD [Description] nvarchar(500) NULL;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'[dbo].[Products]', N'U') AND c.name = N'Description')
                    ALTER TABLE [dbo].[Products] DROP COLUMN [Description];
                """);

            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM sys.columns c WHERE c.object_id = OBJECT_ID(N'[dbo].[Products]', N'U') AND c.name = N'CategoryName')
                BEGIN
                    ALTER TABLE [dbo].[Products] DROP CONSTRAINT IF EXISTS DF_Products_CategoryName;
                    ALTER TABLE [dbo].[Products] DROP COLUMN [CategoryName];
                END
                """);
        }
    }
}
