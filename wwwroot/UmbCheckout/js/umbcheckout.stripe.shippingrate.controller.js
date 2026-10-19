function UmbCheckout($scope, umbCheckoutResources, umbCheckoutStripeResources, $routeParams, notificationsService, formHelper, $location, localizationService, umbCheckoutHelpers) {
    var vm = this;
    vm.shippingRateName;
    vm.deleteButtonState = "init";
    vm.saveButtonState = "init";
    vm.properties = [];
    vm.LicenseState = {};
    vm.stripeShippingRates = [];
    vm.stripeShippingRate = {};
    vm.stripeShippingRateAmount = "";
    vm.stripeShippingRateUrl = "";
    vm.shipping_help = "";

    localizationService.localize("umbcheckoutstripe_shipping_rate").then(function (value) {
        vm.shippingRateName = value;
    });

    vm.clickItem = clickItem;

    function setStripeShippingRate(rate) {
        vm.stripeShippingRate = rate;

        // The API converts the amount on the server (fixedAmountMoney). Stripe's own field is fixed_amount (Stripe.net's
        // System.Text.Json names) or fixedAmount (older Stripe.net); it is only shown raw, as a fallback.
        var fixedAmount = rate ? (rate.fixed_amount || rate.fixedAmount) : null;
        vm.stripeShippingRateAmount = formatMoney(rate ? rate.fixedAmountMoney : null,
            fixedAmount ? fixedAmount.amount : null,
            fixedAmount ? fixedAmount.currency : null);
    }

    // Formats a money object from the server ({ minorUnits, amount, currency }; amount is in major units). Without one (an
    // older API, or no currency) the raw Stripe value is shown, labelled as minor units, with its currency code: never a guessed conversion.
    function formatMoney(money, rawAmount, rawCurrency) {
        if (money && money.amount !== null && money.amount !== undefined && money.currency) {
            try {
                return new Intl.NumberFormat('en', { style: 'currency', currency: money.currency.toUpperCase() }).format(money.amount);
            } catch (e) {
                return money.amount + " " + money.currency.toUpperCase();
            }
        }

        if (rawAmount === null || rawAmount === undefined) {
            return "";
        }

        // Labelled: a raw Stripe amount is in minor units (495 is 4.95 GBP), so a bare "495 GBP" would read as 100 times the value.
        return rawAmount + " (minor units)" + (rawCurrency ? " " + String(rawCurrency).toUpperCase() : "");
    }

    function clickItem(item) {

        vm.properties.forEach(function (v) {
            if (v.alias == "name") v.value = item.display_name; // Stripe.net's own (snake_case) JSON name
            if (v.alias == "value") {
                v.value = item.id

                umbCheckoutStripeResources.getStripeShippingRate(v.value)
                    .then(function (response) {

                        setStripeShippingRate(response.data)
                    })
                    .catch(umbCheckoutHelpers.requestFailed);
            };
        });
    }

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

    umbCheckoutStripeResources.getShippingRate($routeParams.id)
        .then(function (response) {

            vm.properties = response.data.properties

            response.data.properties.forEach(function (v) {
                if (v.alias == "value") {
                    if (v.value) {
                        umbCheckoutStripeResources.getStripeShippingRate(v.value)
                            .then(function (response) {

                                setStripeShippingRate(response.data)
                            })
                            .catch(umbCheckoutHelpers.requestFailed);
                    }
                };
            });
        })
        .catch(function (response) {
            // Settings section users only (403 for anyone else)
            umbCheckoutHelpers.requestFailed(response, "umbcheckout_settings_access_denied_message");
        });

    umbCheckoutStripeResources.getStripeShippingRates()
        .then(function (response) {
            // A Stripe list: the rates are in data, with Stripe.net's own (snake_case) JSON names
            var rates = (response.data && response.data.data) || [];
            angular.forEach(rates, function (value, key) {

                value.name = value.display_name
            });

            vm.stripeShippingRates = rates
        })
        .catch(umbCheckoutHelpers.requestFailed);

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