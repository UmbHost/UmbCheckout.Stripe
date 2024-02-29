function UmbCheckout($scope, umbCheckoutStripeResources, notificationsService, formHelper) {
    var vm = this;
    vm.saveButtonState = "init";
    vm.createFolderError = "";
    vm.properties = [];
    vm.saveConfiguration = saveConfiguration;

    umbCheckoutStripeResources.getStripeSettings()
        .then(function (response) {
            vm.properties = response.data
        }
    );

    function saveConfiguration() {
        vm.saveButtonState = "busy";

        if (formHelper.submitForm({ scope: $scope })) {
            var configurationValues = {};
            angular.forEach(vm.properties, function (value, key) {

                var newKey = value.alias;

                configurationValues[newKey] = value.value
            });

            umbCheckoutStripeResources.updateStripeSettings(configurationValues)
                .then(function (response) {
                    vm.properties = response.data

                    localizationService.localize("umbcheckoutstripe_settings_saved_title").then(function (title) {
                        localizationService.localize("umbcheckoutstripe_settings_saved_message").then(function (message) {
                            notificationsService.success(title, message);
                        });
                    });

                    vm.saveButtonState = "success";
                    $scope.configurationForm.$dirty = false;
                })
                .catch(
                    function (response) {
                        localizationService.localize("umbcheckoutstripe_settings_save_failed_title").then(function (title) {
                            localizationService.localize("umbcheckoutstripe_settings_save_failed_message").then(function (message) {
                                notificationsService.error(title, message);
                            });
                        });

                        vm.saveButtonState = "error";
                    }
                );
        }
        else {
            vm.saveButtonState = "error";
        }
    }
}
angular.module("umbraco").controller("UmbCheckout.Stripe.Settings.Controller", UmbCheckout);