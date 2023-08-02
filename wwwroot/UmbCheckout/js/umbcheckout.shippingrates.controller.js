function UmbCheckout(umbCheckoutResources, $location) {
    var vm = this;
    vm.shippingRates = [];
    vm.LicenseState = {}

    umbCheckoutResources.getLicenseStatus()
        .then(function (response) {
            if (response.data.status == "Invalid" || response.data.status == "Unlicensed") {
                vm.LicenseState.Valid = false;
                vm.LicenseState.Message = "UmbCheckout is running in unlicensed mode, please <a href=\"#\" target=\"_blank\"  class=\"red bold underline\">purchase a license</a> to support development"
            } else if (response.data.status == "Active") {
                vm.LicenseState.Valid = true;
            }
        }
        );

    umbCheckoutResources.getShippingRates()
        .then(function (response) {
            angular.forEach(response.data, function (value, key) {

                value.editPath = "/settings/UmbCheckout/shippingRate/" + value.id
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
        $location.path("/settings/UmbCheckout/shippingRate");
    }
}
angular.module("umbraco").controller("UmbCheckout.ShippingRates.Controller", UmbCheckout);