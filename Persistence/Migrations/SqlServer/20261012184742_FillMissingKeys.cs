using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.SqlServer
{
    /// <summary>Rows created before the Key column existed have a NULL Key, which the backoffice can't look up. Give each its own key.</summary>
    public partial class FillMissingKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE [UmbCheckoutStripeShipping] SET [Key] = NEWID() WHERE [Key] IS NULL;");
            migrationBuilder.Sql("UPDATE [UmbCheckoutStripeSettings] SET [Key] = NEWID() WHERE [Key] IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to undo: the keys stay valid.
        }
    }
}
