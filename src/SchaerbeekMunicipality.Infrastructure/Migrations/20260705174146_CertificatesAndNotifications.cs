using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CertificatesAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificate_requests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    certificate_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    issued_by_officer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificate_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "outbound_notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    registration_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    message = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbound_notifications", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificate_requests_reference_number",
                table: "certificate_requests",
                column: "reference_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_certificate_requests_registration_case_id",
                table: "certificate_requests",
                column: "registration_case_id");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_created_at",
                table: "outbound_notifications",
                column: "created_at");

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_registration_case_id",
                table: "outbound_notifications",
                column: "registration_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificate_requests");

            migrationBuilder.DropTable(
                name: "outbound_notifications");
        }
    }
}
