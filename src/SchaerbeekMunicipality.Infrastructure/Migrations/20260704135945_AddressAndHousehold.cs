using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddressAndHousehold : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "address_box",
                table: "registration_cases",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_house_number",
                table: "registration_cases",
                type: "character varying(16)",
                maxLength: 16,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_municipality",
                table: "registration_cases",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_postal_code",
                table: "registration_cases",
                type: "character varying(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "address_street",
                table: "registration_cases",
                type: "character varying(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "housing_situation",
                table: "registration_cases",
                type: "character varying(64)",
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "civil_status",
                table: "persons",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "marriage_date",
                table: "persons",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "marriage_place",
                table: "persons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "spouse_family_name",
                table: "persons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "spouse_given_name",
                table: "persons",
                type: "character varying(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "households",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_households", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "municipalities",
                columns: table => new
                {
                    postal_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_municipalities", x => x.postal_code);
                });

            migrationBuilder.CreateTable(
                name: "streets",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false),
                    postal_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_streets", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "household_members",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    household_id = table.Column<Guid>(type: "uuid", nullable: false),
                    given_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    family_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    role = table.Column<string>(type: "character varying(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_household_members", x => x.id);
                    table.ForeignKey(
                        name: "fk_household_members_households_household_id",
                        column: x => x.household_id,
                        principalTable: "households",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_household_members_household_id",
                table: "household_members",
                column: "household_id");

            migrationBuilder.CreateIndex(
                name: "ix_households_registration_case_id",
                table: "households",
                column: "registration_case_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_streets_postal_code",
                table: "streets",
                column: "postal_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "household_members");

            migrationBuilder.DropTable(
                name: "municipalities");

            migrationBuilder.DropTable(
                name: "streets");

            migrationBuilder.DropTable(
                name: "households");

            migrationBuilder.DropColumn(
                name: "address_box",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "address_house_number",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "address_municipality",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "address_postal_code",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "address_street",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "housing_situation",
                table: "registration_cases");

            migrationBuilder.DropColumn(
                name: "civil_status",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "marriage_date",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "marriage_place",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "spouse_family_name",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "spouse_given_name",
                table: "persons");
        }
    }
}
