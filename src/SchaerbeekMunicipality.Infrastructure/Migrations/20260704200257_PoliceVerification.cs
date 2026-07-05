using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PoliceVerification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "police_verification_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    result = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    officer_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    attempt_number = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_police_verification_requests", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_police_verification_requests_completed_at",
                table: "police_verification_requests",
                column: "completed_at");

            migrationBuilder.CreateIndex(
                name: "ix_police_verification_requests_registration_case_id",
                table: "police_verification_requests",
                column: "registration_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "police_verification_requests");
        }
    }
}
