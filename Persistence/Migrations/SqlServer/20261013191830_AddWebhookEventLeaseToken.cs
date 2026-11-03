using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class AddWebhookEventLeaseToken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeaseToken",
                table: "UmbCheckoutStripeWebhookEvents",
                type: "nvarchar(64)",
                maxLength: 64,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeaseToken",
                table: "UmbCheckoutStripeWebhookEvents");
        }
    }
}
