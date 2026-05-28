using Market.Infrastructure.Database;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations;

[DbContext(typeof(DatabaseContext))]
[Migration("20260528100000_Products_AddProductVariants")]
public partial class Products_AddProductVariants : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ProductVariants' AND schema_id = SCHEMA_ID(N'dbo'))
            BEGIN
                CREATE TABLE [dbo].[ProductVariants] (
                    [Id] int NOT NULL IDENTITY,
                    [ProductId] int NOT NULL,
                    [Size] nvarchar(50) NOT NULL,
                    [Color] nvarchar(100) NOT NULL,
                    [Price] decimal(18,2) NOT NULL,
                    [StockQuantity] int NOT NULL,
                    [IsDeleted] bit NOT NULL,
                    [CreatedAtUtc] datetime2 NOT NULL,
                    [ModifiedAtUtc] datetime2 NULL,
                    CONSTRAINT [PK_ProductVariants] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_ProductVariants_Products_ProductId] FOREIGN KEY ([ProductId])
                        REFERENCES [dbo].[Products] ([Id]) ON DELETE CASCADE
                );

                CREATE INDEX [IX_ProductVariants_ProductId] ON [dbo].[ProductVariants] ([ProductId]);
                CREATE UNIQUE INDEX [IX_ProductVariants_ProductId_Size_Color]
                    ON [dbo].[ProductVariants] ([ProductId], [Size], [Color]);
            END
            """);

        migrationBuilder.Sql("""
            INSERT INTO [dbo].[ProductVariants] (
                [ProductId], [Size], [Color], [Price], [StockQuantity], [IsDeleted], [CreatedAtUtc])
            SELECT
                p.[Id],
                N'Standard',
                N'Standard',
                p.[Price],
                p.[StockQuantity],
                0,
                GETUTCDATE()
            FROM [dbo].[Products] p
            WHERE p.[IsDeleted] = 0
              AND NOT EXISTS (
                  SELECT 1
                  FROM [dbo].[ProductVariants] v
                  WHERE v.[ProductId] = p.[Id]);
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("""
            IF EXISTS (SELECT 1 FROM sys.tables WHERE name = N'ProductVariants' AND schema_id = SCHEMA_ID(N'dbo'))
                DROP TABLE [dbo].[ProductVariants];
            """);
    }
}
