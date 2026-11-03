using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.Sqlite
{
    /// <inheritdoc />
    public partial class AddWebhookEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UmbCheckoutStripeWebhookEvents",
                columns: table => new
                {
                    EventId = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    ReceivedUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LeaseUntilUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbCheckoutStripeWebhookEvents", x => x.EventId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UmbCheckoutStripeWebhookEvents_ReceivedUtc",
                table: "UmbCheckoutStripeWebhookEvents",
                column: "ReceivedUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UmbCheckoutStripeWebhookEvents");
        }
    }
}
