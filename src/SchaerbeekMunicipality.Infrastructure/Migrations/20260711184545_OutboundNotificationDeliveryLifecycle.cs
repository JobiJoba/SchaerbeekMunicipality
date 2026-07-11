using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchaerbeekMunicipality.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class OutboundNotificationDeliveryLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "attempt_count",
                table: "outbound_notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "delivery_status",
                table: "outbound_notifications",
                type: "character varying(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "Sent");

            migrationBuilder.AddColumn<string>(
                name: "last_error",
                table: "outbound_notifications",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "next_attempt_at",
                table: "outbound_notifications",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "sent_at",
                table: "outbound_notifications",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE outbound_notifications
                SET sent_at = created_at,
                    next_attempt_at = created_at
                WHERE sent_at IS NULL;
                """);

            migrationBuilder.CreateIndex(
                name: "ix_outbound_notifications_delivery_status_next_attempt_at",
                table: "outbound_notifications",
                columns: new[] { "delivery_status", "next_attempt_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_outbound_notifications_delivery_status_next_attempt_at",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "attempt_count",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "delivery_status",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "last_error",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "next_attempt_at",
                table: "outbound_notifications");

            migrationBuilder.DropColumn(
                name: "sent_at",
                table: "outbound_notifications");
        }
    }
}
