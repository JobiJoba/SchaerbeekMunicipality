using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Phase14PassportIdRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "document_request_case_id",
                table: "administrative_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "document_request_cases",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    person_id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    assigned_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_by_officer_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    photo_document_id = table.Column<Guid>(type: "uuid", nullable: true),
                    photo_attached = table.Column<bool>(type: "boolean", nullable: false),
                    fee_paid = table.Column<bool>(type: "boolean", nullable: false),
                    fee_payment_reference = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    issued_document_number = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_document_request_cases", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_administrative_documents_document_request_case_id",
                table: "administrative_documents",
                column: "document_request_case_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "document_request_cases");

            migrationBuilder.DropIndex(
                name: "ix_administrative_documents_document_request_case_id",
                table: "administrative_documents");

            migrationBuilder.DropColumn(
                name: "document_request_case_id",
                table: "administrative_documents");
        }
    }
}
