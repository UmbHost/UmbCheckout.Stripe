function UmbCheckout($scope, umbCheckoutResources, umbCheckoutStripeResources, $routeParams, notificationsService, formHelper, $location, localizationService) {
    var vm = this;
    vm.shippingRateName;
    vm.deleteButtonState = "init";
    vm.saveButtonState = "init";
    vm.properties = [];
    vm.LicenseState = {};
    vm.stripeShippingRates = [];
    vm.stripeShippingRate = {};
    vm.stripeShippingRateUrl = "";
    vm.shipping_help = "";

    localizationService.localize("umbcheckoutstripe_shipping_rate").then(function (value) {
        vm.shippingRateName = value;
    });

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
            if (response.data.status == "Invalid" || response.data.status == "Unlicensed" || response.data.status == "Expired") {
                vm.LicenseState.Valid = false;

                localizationService.localize("umbcheckout_unlicensed_warning").then(function (value) {
                    vm.LicenseState.Message = value;
                });

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

    umbCheckoutStripeResources.getStripeSettings()
        .then(function (response) {

            if (response.data.useLiveApiDetails) {
                vm.stripeShippingRateUrl = "https://dashboard.stripe.com/shipping-rates/";
            } else {
                vm.stripeShippingRateUrl = "https://dashboard.stripe.com/test/shipping-rates/";
            }

            localizationService.localize("umbcheckoutstripe_shipping_help").then(function (message) {
                vm.shipping_help = message.replace("[[STRIPE_URL]]", vm.stripeShippingRateUrl);
            });
        }
    );

    localizationService.localize("umbcheckoutstripe_shipping_id").then(function (value) {
        vm.options = {
            includeProperties: [
                { alias: "id", header: value }
            ]
        };

    });

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

                        localizationService.localize("umbcheckoutstripe_shipping_rate_notification_updated_title").then(function (title) {
                            localizationService.localize("umbcheckoutstripe_shipping_rate_notification_updated_message").then(function (message) {
                                notificationsService.success(title, message);
                            });

                        });

                        vm.saveButtonState = "success";
                        $scope.shippingRateForm.$dirty = false;
                    })
                    .catch(
                        function (response) {

                            localizationService.localize("umbcheckoutstripe_shipping_rate_notification_update_failed_title").then(function (title) {
                                localizationService.localize("umbcheckoutstripe_shipping_rate_notification_update_failed_message").then(function (message) {
                                    notificationsService.error(title, message);
                                });
                            });

                            vm.saveButtonState = "error";
                        }
                    );
            } else {
                umbCheckoutStripeResources.createShippingRate(configurationValues)
                    .then(function (response) {
                        vm.properties = response.data.properties

                        localizationService.localize("umbcheckoutstripe_shipping_rate_notification_created_title").then(function (title) {
                            localizationService.localize("umbcheckoutstripe_shipping_rate_notification_created_message").then(function (message) {
                                notificationsService.success(title, message);
                            });
                        });

                        vm.saveButtonState = "success";
                        $scope.shippingRateForm.$dirty = false;
                        $location.path("/settings/UmbCheckout/StripeShippingRate/" + response.data.key);
                    })
                    .catch(
                        function (response) {

                            localizationService.localize("umbcheckoutstripe_shipping_rate_notification_create_failed_title").then(function (title) {
                                localizationService.localize("umbcheckoutstripe_shipping_rate_notification_create_failed_message").then(function (message) {
                                    notificationsService.error(title, message);
                                });
                            });

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

                localizationService.localize("umbcheckoutstripe_shipping_rate_notification_deleted_title").then(function (title) {
                    localizationService.localize("umbcheckoutstripe_shipping_rate_notification_deleted_message").then(function (message) {
                        notificationsService.success(title, message);
                    });
                });

                vm.saveButtonState = "success";
                $location.path("/settings/UmbCheckout/StripeShippingRates");
            })
            .catch(
                function (response) {
                    localizationService.localize("umbcheckoutstripe_shipping_rate_notification_delete_failed_title").then(function (title) {
                        localizationService.localize("umbcheckoutstripe_shipping_rate_notification_delete_failed_message").then(function (message) {
                            notificationsService.error(title, message);
                        });
                    });

                    vm.saveButtonState = "error";
                }
            );
    }

    vm.deleteShippingRate = deleteShippingRate;
}
angular.module("umbraco").controller("UmbCheckout.Stripe.ShippingRate.Controller", UmbCheckout);