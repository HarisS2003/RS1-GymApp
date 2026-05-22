using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Market.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDemotedTrainerUserRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                UPDATE u
                SET u.RoleId = 3,
                    u.ModifiedAtUtc = SYSUTCDATETIME()
                FROM [Users] u
                INNER JOIN [Trainers] t ON t.UserId = u.Id
                WHERE t.IsDeleted = 1
                  AND u.RoleId = 2
                  AND u.IsDeleted = 0
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
