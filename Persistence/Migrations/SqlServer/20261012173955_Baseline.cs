using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class Baseline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UmbCheckoutStripeSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UseLiveApiDetails = table.Column<bool>(type: "bit", nullable: false),
                    CollectPhoneNumber = table.Column<bool>(type: "bit", nullable: false),
                    CollectPromotionalConsent = table.Column<bool>(type: "bit", nullable: false),
                    AllowPromotionalCodes = table.Column<bool>(type: "bit", nullable: false),
                    ShippingAllowedCountries = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EnableAbandonedCartRecovery = table.Column<bool>(type: "bit", nullable: false),
                    AllowPromotionalCodesOnRecoveredCarts = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbCheckoutStripeSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmbCheckoutStripeShipping",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Key = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbCheckoutStripeShipping", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_shippingKey",
                table: "UmbCheckoutStripeShipping",
                column: "Key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stripeIdShippingRate",
                table: "UmbCheckoutStripeShipping",
                column: "Value",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UmbCheckoutStripeSettings");

            migrationBuilder.DropTable(
                name: "UmbCheckoutStripeShipping");
        }
    }
}
