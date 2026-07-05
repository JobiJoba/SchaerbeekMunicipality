using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoleBoundariesAndCaseLocking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "assigned_officer_id",
                table: "registration_cases",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "locked_at",
                table: "registration_cases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "locked_by_officer_id",
                table: "registration_cases",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE registration_cases
                SET locked_by_officer_id = assigned_officer_id,
                    locked_at = opened_at
                WHERE assigned_officer_id IS NOT NULL
                  AND status IN ('Intake', 'UnderReview', 'AwaitingPoliceVerification', 'Approved', 'Suspended');
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "locked_at",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "locked_by_officer_id",
                table: "registration_cases");

            migrationBuilder.AlterColumn<Guid>(
                name: "assigned_officer_id",
                table: "registration_cases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
