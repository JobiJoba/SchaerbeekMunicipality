using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ResidenceCategoryAndPermits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "immigration_decision_date",
                table: "registration_cases",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "immigration_decision_reference",
                table: "registration_cases",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "residence_category",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "residence_permits",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permit_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    card_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_until = table.Column<DateOnly>(type: "date", nullable: false),
                    issuing_authority = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_residence_permits", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_residence_permits_registration_case_id",
                table: "residence_permits",
                column: "registration_case_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "residence_permits");

            migrationBuilder.DropColumn(
                name: "immigration_decision_date",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "immigration_decision_reference",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "residence_category",
                table: "registration_cases");
        }
    }
}
