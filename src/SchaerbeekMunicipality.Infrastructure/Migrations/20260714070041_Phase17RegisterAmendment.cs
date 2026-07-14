using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase17RegisterAmendment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "register_amendment_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "register_amendment_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amendment_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    assigned_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    proposed_given_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    proposed_family_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    proposed_nationality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    proposed_civil_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    proposed_spouse_given_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    proposed_spouse_family_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    proposed_marriage_date = table.Column<DateOnly>(type: "date", nullable: true),
                    proposed_marriage_place = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    proposed_marriage_recognition_status = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    proposed_changes_recorded = table.Column<bool>(type: "boolean", nullable: false),
                    supporting_document_attached = table.Column<bool>(type: "boolean", nullable: false),
                    opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    decision_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    applied_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    decision_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_register_amendment_cases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_register_amendment_case_id",
                table: "administrative_documents",
                column: "register_amendment_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_register_amendment_cases_person_id",
                table: "register_amendment_cases",
                column: "person_id");

            migrationBuilder.CreateIndex(
                name: "ix_register_amendment_cases_status",
                table: "register_amendment_cases",
                column: "status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "register_amendment_cases");

            migrationBuilder.DropIndex(
                name: "ix_administrative_documents_register_amendment_case_id",
                table: "administrative_documents");

            migrationBuilder.DropColumn(
                name: "register_amendment_case_id",
                table: "administrative_documents");
        }
    }
}
