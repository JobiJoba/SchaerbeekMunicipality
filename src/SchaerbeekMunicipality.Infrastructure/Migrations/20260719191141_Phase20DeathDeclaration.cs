using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase20DeathDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "date_of_death",
                table: "persons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "death_declaration_case_id",
                table: "outbound_notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "death_declaration_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "death_declaration_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    assigned_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    death_date = table.Column<DateOnly>(type: "date", nullable: true),
                    death_place = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    death_abroad = table.Column<bool>(type: "boolean", nullable: false),
                    informant_relationship = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    death_act_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    household_reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    person_identified = table.Column<bool>(type: "boolean", nullable: false),
                    death_facts_recorded = table.Column<bool>(type: "boolean", nullable: false),
                    death_act_attached = table.Column<bool>(type: "boolean", nullable: false),
                    household_reviewed = table.Column<bool>(type: "boolean", nullable: false),
                    opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    decision_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    suspension_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    decision_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status_before_suspension = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_death_declaration_cases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_death_declaration_case_id",
                table: "outbound_notifications",
                column: "death_declaration_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_death_declaration_case_id",
                table: "administrative_documents",
                column: "death_declaration_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_death_declaration_cases_person_id",
                table: "death_declaration_cases",
                column: "person_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "death_declaration_cases");

            migrationBuilder.DropIndex(
                name: "ix_outbound_notifications_death_declaration_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropIndex(
                name: "ix_administrative_documents_death_declaration_case_id",
                table: "administrative_documents");

            migrationBuilder.DropColumn(
                name: "date_of_death",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "death_declaration_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "death_declaration_case_id",
                table: "administrative_documents");
        }
    }
}
