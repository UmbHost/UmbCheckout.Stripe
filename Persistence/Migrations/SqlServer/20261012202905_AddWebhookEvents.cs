using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.SqlServer
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
                    EventId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ReceivedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    LeaseUntilUtc = table.Column<DateTime>(type: "datetime2", nullable: true)
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
