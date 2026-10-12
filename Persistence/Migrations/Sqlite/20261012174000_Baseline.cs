using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.Sqlite
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
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<Guid>(type: "TEXT", nullable: true),
                    UseLiveApiDetails = table.Column<bool>(type: "INTEGER", nullable: false),
                    CollectPhoneNumber = table.Column<bool>(type: "INTEGER", nullable: false),
                    CollectPromotionalConsent = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowPromotionalCodes = table.Column<bool>(type: "INTEGER", nullable: false),
                    ShippingAllowedCountries = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true, collation: "NOCASE"),
                    EnableAbandonedCartRecovery = table.Column<bool>(type: "INTEGER", nullable: false),
                    AllowPromotionalCodesOnRecoveredCarts = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbCheckoutStripeSettings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UmbCheckoutStripeShipping",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Key = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "NOCASE"),
                    Value = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false, collation: "NOCASE")
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
