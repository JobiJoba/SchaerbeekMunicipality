using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DecisionAndRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "closed_at",
                table: "registration_cases",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "decision_notes",
                table: "registration_cases",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "decision_officer_id",
                table: "registration_cases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "register_target",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rejection_reason",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "status_before_suspension",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "suspension_reason",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "case_audit_entries",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    officer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    details = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_case_audit_entries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_case_audit_entries_case_id",
                table: "case_audit_entries",
                column: "case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "case_audit_entries");

            migrationBuilder.DropColumn(
                name: "closed_at",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "decision_notes",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "decision_officer_id",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "register_target",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "rejection_reason",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "status_before_suspension",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "suspension_reason",
                table: "registration_cases");
        }
    }
}
