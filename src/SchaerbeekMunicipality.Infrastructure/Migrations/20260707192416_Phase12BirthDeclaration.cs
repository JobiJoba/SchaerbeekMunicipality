using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase12BirthDeclaration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "outbound_notifications",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "birth_declaration_case_id",
                table: "outbound_notifications",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "birth_declaration_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "birth_declaration_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    assigned_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    child_given_names = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    child_family_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    child_sex = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    child_date_of_birth = table.Column<DateOnly>(type: "date", nullable: true),
                    child_time_of_birth = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    child_place_of_birth = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    medical_declaration_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    household_street = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    household_house_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    household_box = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: true),
                    household_postal_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: true),
                    household_municipality = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    child_details_recorded = table.Column<bool>(type: "boolean", nullable: false),
                    at_least_one_parent_linked = table.Column<bool>(type: "boolean", nullable: false),
                    medical_declaration_attached = table.Column<bool>(type: "boolean", nullable: false),
                    household_established = table.Column<bool>(type: "boolean", nullable: false),
                    opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    confirmed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    child_person_id = table.Column<Guid>(type: "uuid", nullable: true),
                    child_national_register_number = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: true),
                    decision_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    suspension_reason = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    decision_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status_before_suspension = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_birth_declaration_cases", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "birth_declaration_parent_links",
                columns: table => new
                {
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    birth_declaration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_birth_declaration_parent_links", x => new { x.birth_declaration_case_id, x.person_id });
                    table.ForeignKey(
                        name: "fk_birth_declaration_parent_links_birth_declaration_cases_birt",
                        column: x => x.birth_declaration_case_id,
                        principalTable: "birth_declaration_cases",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_birth_declaration_case_id",
                table: "outbound_notifications",
                column: "birth_declaration_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_birth_declaration_case_id",
                table: "administrative_documents",
                column: "birth_declaration_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "birth_declaration_parent_links");

            migrationBuilder.DropTable(
                name: "birth_declaration_cases");

            migrationBuilder.DropIndex(
                name: "ix_outbound_notifications_birth_declaration_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropIndex(
                name: "ix_administrative_documents_birth_declaration_case_id",
                table: "administrative_documents");

            migrationBuilder.DropColumn(
                name: "birth_declaration_case_id",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "birth_declaration_case_id",
                table: "administrative_documents");

            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "outbound_notifications",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "registration_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
