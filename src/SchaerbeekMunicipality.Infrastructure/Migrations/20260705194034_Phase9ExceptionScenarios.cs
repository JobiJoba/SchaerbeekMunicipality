using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase9ExceptionScenarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "birth_evidence_established",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "duplicate_investigation_resolved",
                table: "registration_cases",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "duplicate_investigation_status",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "birth_country",
                table: "persons",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "birth_place",
                table: "persons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "marriage_recognition_status",
                table: "persons",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "birth_evidence_established",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "duplicate_investigation_resolved",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "duplicate_investigation_status",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "birth_country",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "birth_place",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "marriage_recognition_status",
                table: "persons");
        }
    }
}
