using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UmbCheckout.Stripe.Persistence.Migrations.Sqlite
{
    /// <summary>
    /// Rows created before the Key column existed have a NULL Key, which the backoffice can't look up. Give each its own key
    /// (random, in EF's upper-case text Guid format).
    /// </summary>
    public partial class FillMissingKeys : Migration
    {
        private const string NewKey =
            "upper(hex(randomblob(4))) || '-' || upper(hex(randomblob(2))) || '-' || upper(hex(randomblob(2))) || '-' || upper(hex(randomblob(2))) || '-' || upper(hex(randomblob(6)))";

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($"UPDATE \"UmbCheckoutStripeShipping\" SET \"Key\" = {NewKey} WHERE \"Key\" IS NULL;");
            migrationBuilder.Sql($"UPDATE \"UmbCheckoutStripeSettings\" SET \"Key\" = {NewKey} WHERE \"Key\" IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Nothing to undo: the keys stay valid.
        }
    }
}
