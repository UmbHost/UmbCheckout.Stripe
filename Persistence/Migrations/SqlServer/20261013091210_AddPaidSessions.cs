using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.SqlServer
{
    /// <inheritdoc />
    public partial class AddPaidSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UmbCheckoutStripePaidSessions",
                columns: table => new
                {
                    CheckoutSessionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PaidUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UmbCheckoutStripePaidSessions", x => x.CheckoutSessionId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UmbCheckoutStripePaidSessions_PaidUtc",
                table: "UmbCheckoutStripePaidSessions",
                column: "PaidUtc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UmbCheckoutStripePaidSessions");
        }
    }
}
