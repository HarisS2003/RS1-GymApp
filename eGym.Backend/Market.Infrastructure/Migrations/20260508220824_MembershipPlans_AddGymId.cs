using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MembershipPlans_AddGymId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Idempotent: same DB may already have GymId from GymDatabaseContext migration or manual change.
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.columns c
                    WHERE c.object_id = OBJECT_ID(N'[dbo].[MembershipPlans]', N'U')
                      AND c.name = N'GymId')
                BEGIN
                    ALTER TABLE [dbo].[MembershipPlans] ADD [GymId] int NULL;
                END
                """);

            migrationBuilder.Sql("""
                UPDATE [MembershipPlans]
                SET [GymId] = (SELECT TOP (1) [Id] FROM [Gyms] ORDER BY [Id])
                WHERE [GymId] IS NULL
                  AND EXISTS (SELECT 1 FROM [Gyms]);
                """);

            migrationBuilder.Sql("""
                IF EXISTS (SELECT 1 FROM [MembershipPlans] WHERE [GymId] IS NULL)
                    THROW 50000, N'MembershipPlans require GymId: add at least one Gym before migrating, or remove orphan membership plans.', 1;
                """);

            migrationBuilder.Sql("""
                IF EXISTS (
                    SELECT 1
                    FROM sys.columns c
                    WHERE c.object_id = OBJECT_ID(N'[dbo].[MembershipPlans]', N'U')
                      AND c.name = N'GymId'
                      AND c.is_nullable = 1)
                BEGIN
                    ALTER TABLE [dbo].[MembershipPlans] ALTER COLUMN [GymId] INT NOT NULL;
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes i
                    WHERE i.name = N'IX_MembershipPlans_GymId'
                      AND i.object_id = OBJECT_ID(N'[dbo].[MembershipPlans]', N'U'))
                BEGIN
                    CREATE NONCLUSTERED INDEX [IX_MembershipPlans_GymId]
                    ON [dbo].[MembershipPlans]([GymId]);
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_MembershipPlans_Gyms_GymId')
                BEGIN
                    ALTER TABLE [dbo].[MembershipPlans] WITH CHECK ADD CONSTRAINT [FK_MembershipPlans_Gyms_GymId]
                        FOREIGN KEY ([GymId]) REFERENCES [dbo].[Gyms]([Id]) ON DELETE NO ACTION;
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MembershipPlans_Gyms_GymId",
                table: "MembershipPlans");

            migrationBuilder.DropIndex(
                name: "IX_MembershipPlans_GymId",
                table: "MembershipPlans");

            migrationBuilder.DropColumn(
                name: "GymId",
                table: "MembershipPlans");
        }
    }
}
