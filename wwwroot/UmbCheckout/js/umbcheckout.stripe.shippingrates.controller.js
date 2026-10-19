function UmbCheckout(umbCheckoutResources, umbCheckoutStripeResources, $location, localizationService, umbCheckoutHelpers) {
    var vm = this;
    vm.shippingRatesName;
    vm.shippingRates = [];
    vm.LicenseState = {};
    vm.shipping_help = "";

    localizationService.localize("umbcheckoutstripe_shipping_rates").then(function (value) {
        vm.shippingRatesName = value;
    });

    umbCheckoutResources.getLicenseStatus()
        .then(function (response) {
            // A paid add-on installed: unlicensed shows the red warning; without one, an info bar.
            vm.LicenseState.AddOnsInstalled = response.data.addOnsInstalled === true;
            if (response.data.status != "Active" && response.data.status != "Development") { // anything else (Invalid, Expired, Suspended, Unlicensed, Domain not licensed) is unlicensed
                vm.LicenseState.Valid = false;

                localizationService.localize("umbcheckout_unlicensed_warning").then(function (value) {
                    vm.LicenseState.Message = value;
                });

            } else {
                vm.LicenseState.Valid = true;
            }
        }
        );

    umbCheckoutStripeResources.getShippingRates()
        .then(function (response) {
            angular.forEach(response.data, function (value, key) {

                value.editPath = "/settings/UmbCheckout/StripeShippingRate/" + value.key
            });

            vm.shippingRates = response.data
        })
        .catch(function (response) {
            // Settings section users only (403 for anyone else)
            umbCheckoutHelpers.requestFailed(response, "umbcheckout_settings_access_denied_message");
        });

    localizationService.localize("general_value").then(function (v) {
        vm.options = {
            includeProperties: [
                { alias: "value", header: v }
            ]
        };
    });

    vm.clickItem = clickItem;

    function clickItem(item) {
        $location.path(item.editPath);
    }

    vm.clickCreateButton = clickCreateButton;

    function clickCreateButton() {
        $location.path("/settings/UmbCheckout/StripeShippingRate");
    }

    umbCheckoutStripeResources.getStripeSettings()
        .then(function (response) {

            // The settings API returns a property list; useLiveApiDetails is one of its properties
            if (umbCheckoutHelpers.isLiveMode(response.data)) {
                vm.stripeShippingRateUrl = "https://dashboard.stripe.com/shipping-rates/";
            } else {
                vm.stripeShippingRateUrl = "https://dashboard.stripe.com/test/shipping-rates/";
            }

            localizationService.localize("umbcheckoutstripe_shipping_help").then(function (message) {
                vm.shipping_help = message.replace("[[STRIPE_URL]]", vm.stripeShippingRateUrl);
            });
        }
        );
}
angular.module("umbraco").controller("UmbCheckout.Stripe.ShippingRates.Controller", UmbCheckout);