using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NationalRegisterSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bis_number",
                table: "persons",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "linked_register_record_id",
                table: "persons",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "national_register_number",
                table: "persons",
                type: "character varying(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "national_register_persons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    given_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    family_name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    nationality = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    bis_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true),
                    national_register_number = table.Column<string>(type: "character varying(11)", maxLength: 11, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_national_register_persons", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_persons_linked_register_record_id",
                table: "persons",
                column: "linked_register_record_id");

            migrationBuilder.CreateIndex(
                name: "ix_persons_national_register_number",
                table: "persons",
                column: "national_register_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_national_register_persons_bis_number",
                table: "national_register_persons",
                column: "bis_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_national_register_persons_national_register_number",
                table: "national_register_persons",
                column: "national_register_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "national_register_persons");

            migrationBuilder.DropIndex(
                name: "ix_persons_linked_register_record_id",
                table: "persons");

            migrationBuilder.DropIndex(
                name: "ix_persons_national_register_number",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "bis_number",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "linked_register_record_id",
                table: "persons");

            migrationBuilder.DropColumn(
                name: "national_register_number",
                table: "persons");
        }
    }
}
