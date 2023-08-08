function UmbCheckout(umbCheckoutResources, umbCheckoutStripeResources, $location, localizationService) {
    var vm = this;
    vm.shippingRatesName;
    vm.shippingRates = [];
    vm.LicenseState = {}

    localizationService.localize("umbcheckoutstripe_shipping_rates").then(function (value) {
        vm.shippingRatesName = value;
    });

    umbCheckoutResources.getLicenseStatus()
        .then(function (response) {
            if (response.data.status == "Invalid" || response.data.status == "Unlicensed") {
                vm.LicenseState.Valid = false;
            } else if (response.data.status == "Active") {
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
        }
    );

    vm.options = {
        includeProperties: [
            { alias: "value", header: "Value" }
        ]
    };

    vm.clickItem = clickItem;

    function clickItem(item) {
        $location.path(item.editPath);
    }

    vm.clickCreateButton = clickCreateButton;

    function clickCreateButton() {
        $location.path("/settings/UmbCheckout/StripeShippingRate");
    }
}
angular.module("umbraco").controller("UmbCheckout.Stripe.ShippingRates.Controller", UmbCheckout);