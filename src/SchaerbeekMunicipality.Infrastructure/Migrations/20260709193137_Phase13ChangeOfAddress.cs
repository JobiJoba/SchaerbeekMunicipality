using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase13ChangeOfAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "police_verification_requests",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "change_of_address_case_id",
                table: "police_verification_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicile_box",
                table: "persons",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicile_house_number",
                table: "persons",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicile_municipality",
                table: "persons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicile_postal_code",
                table: "persons",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "domicile_street",
                table: "persons",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "change_of_address_case_id",
                table: "outbound_notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "change_of_address_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "change_of_address_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    assigned_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    previous_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    previous_house_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    previous_box = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    previous_postal_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    previous_municipality = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    new_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    new_house_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    new_box = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    new_postal_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    new_municipality = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    housing_situation = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    housing_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    police_verification_request_id = table.Column<Guid>(type: "uuid", nullable: true),
                    effective_date = table.Column<DateOnly>(type: "date", nullable: true),
                    person_identified = table.Column<bool>(type: "boolean", nullable: false),
                    new_address_declared = table.Column<bool>(type: "boolean", nullable: false),
                    housing_document_attached = table.Column<bool>(type: "boolean", nullable: false),
                    housing_document_required = table.Column<bool>(type: "boolean", nullable: false),
                    police_verification_requested = table.Column<bool>(type: "boolean", nullable: false),
                    police_verification_positive = table.Column<bool>(type: "boolean", nullable: false),
                    opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    decision_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    decision_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_change_of_address_cases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "change_of_address_household_member_links",
                columns: table => new
                {
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    change_of_address_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_change_of_address_household_member_links", x => new { x.change_of_address_case_id, x.person_id });
                    table.ForeignKey(
                        name: "fk_change_of_address_household_member_links_change_of_address_",
                        column: x => x.change_of_address_case_id,
                        principalTable: "change_of_address_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_police_verification_requests_change_of_address_case_id",
                table: "police_verification_requests",
                column: "change_of_address_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_change_of_address_case_id",
                table: "outbound_notifications",
                column: "change_of_address_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_change_of_address_case_id",
                table: "administrative_documents",
                column: "change_of_address_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "change_of_address_household_member_links");

            migrationBuilder.DropTable(
                name: "change_of_address_cases");

            migrationBuilder.DropIndex(
                name: "ix_police_verification_requests_change_of_address_case_id",
                table: "police_verification_requests");

            migrationBuilder.DropIndex(
                name: "ix_outbound_notifications_change_of_address_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropIndex(
                name: "ix_administrative_documents_change_of_address_case_id",
                table: "administrative_documents");

            migrationBuilder.DropColumn(
                name: "change_of_address_case_id",
                table: "police_verification_requests");

            migrationBuilder.DropColumn(
                name: "domicile_box",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "domicile_house_number",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "domicile_municipality",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "domicile_postal_code",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "domicile_street",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "change_of_address_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "change_of_address_case_id",
                table: "administrative_documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "police_verification_requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
