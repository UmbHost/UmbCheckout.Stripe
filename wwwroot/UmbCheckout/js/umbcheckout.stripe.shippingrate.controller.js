function UmbCheckout($scope, umbCheckoutResources, umbCheckoutStripeResources, $routeParams, notificationsService, formHelper, $location) {
    var vm = this;
    vm.deleteButtonState = "init";
    vm.saveButtonState = "init";
    vm.properties = [];
    vm.LicenseState = {};
    vm.stripeShippingRates = [];
    vm.stripeShippingRate = {};

    vm.clickItem = clickItem;

    function clickItem(item) {

        vm.properties.forEach(function (v) {
            if (v.alias == "name") v.value = item.displayName;
            if (v.alias == "value") {
                v.value = item.id

                umbCheckoutStripeResources.getStripeShippingRate(v.value)
                    .then(function (response) {

                        vm.stripeShippingRate = response.data
                    })
            };
        });
    }

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

    umbCheckoutStripeResources.getShippingRate($routeParams.id)
        .then(function (response) {

            vm.properties = response.data.properties

            response.data.properties.forEach(function (v) {
                if (v.alias == "value") {
                    if (v.value) {
                        umbCheckoutStripeResources.getStripeShippingRate(v.value)
                            .then(function (response) {

                                vm.stripeShippingRate = response.data
                            })
                    }
                };
            });
        }
    );

    umbCheckoutStripeResources.getStripeShippingRates()
        .then(function (response) {
            angular.forEach(response.data, function (value, key) {

                value.name = value.displayName
            });

            vm.stripeShippingRates = response.data
        }
        );

    vm.options = {
        includeProperties: [
            { alias: "id", header: "ID" }
        ]
    };

    function saveShippingRate() {
        vm.saveButtonState = "busy";

        if (formHelper.submitForm({ scope: $scope })) {
            var configurationValues = {};
            angular.forEach(vm.properties, function (value, key) {

                var newKey = value.alias;

                configurationValues[newKey] = value.value
            });

            if ($routeParams.id) {
                umbCheckoutStripeResources.updateShippingRate(configurationValues, $routeParams.id)
                    .then(function (response) {
                        vm.properties = response.data.properties
                        notificationsService.success("Shipping Rate updated", "The Shipping Rate has been updated successfully");
                        vm.saveButtonState = "success";
                        $scope.shippingRateForm.$dirty = false;
                    })
                    .catch(
                        function (response) {
                            notificationsService.error("Shipping Rate failed to update", "There was an issue trying to update the Shipping Rate");
                            vm.saveButtonState = "error";
                        }
                    );
            } else {
                umbCheckoutStripeResources.createShippingRate(configurationValues)
                    .then(function (response) {
                        vm.properties = response.data.properties
                        notificationsService.success("Shipping Rate created", "The Shipping Rate has been created successfully");
                        vm.saveButtonState = "success";
                        $scope.shippingRateForm.$dirty = false;
                        $location.path("/settings/UmbCheckout/StripeShippingRate/" + response.data.key);
                    })
                    .catch(
                        function (response) {
                            notificationsService.error("Shipping Rate failed to create", "There was an issue trying to create the Shipping Rate");
                            vm.saveButtonState = "error";
                        }
                    );
            }
        }
        else {
            vm.saveButtonState = "error";
        }
    }

    vm.saveShippingRate = saveShippingRate;

    function deleteShippingRate() {
        vm.deleteButtonState = "busy";

        umbCheckoutStripeResources.deleteShippingRate($routeParams.id)
            .then(function (response) {
                vm.properties = response.data
                notificationsService.success("Shipping Rate deleted", "The Shipping Rate has been deleted successfully");
                vm.saveButtonState = "success";
                $location.path("/settings/UmbCheckout/StripeShippingRates");
            })
            .catch(
                function (response) {
                    notificationsService.error("Shipping Rate failed to delete", "There was an issue trying to delete the Shipping Rate");
                    vm.saveButtonState = "error";
                }
            );
    }

    vm.deleteShippingRate = deleteShippingRate;
}
angular.module("umbraco").controller("UmbCheckout.Stripe.ShippingRate.Controller", UmbCheckout);