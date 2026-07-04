using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CaseIntakeAndIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "address_confirmed",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "address_declared",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "assigned_officer_id",
                table: "registration_cases",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "identity_established",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "legal_residence_established",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "opened_at",
                table: "registration_cases",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<Guid>(
                name: "person_id",
                table: "registration_cases",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "register_determinable",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "visit_reason",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "administrative_documents",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    file_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    storage_path = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    content_hash = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    uploaded_by_officer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    uploaded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_administrative_documents", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "persons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    given_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    family_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    nationality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_persons", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_registration_case_id",
                table: "administrative_documents",
                column: "registration_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "administrative_documents");

            migrationBuilder.DropTable(
                name: "persons");

            migrationBuilder.DropColumn(
                name: "address_confirmed",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "address_declared",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "assigned_officer_id",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "identity_established",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "legal_residence_established",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "opened_at",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "person_id",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "register_determinable",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "visit_reason",
                table: "registration_cases");
        }
    }
}
