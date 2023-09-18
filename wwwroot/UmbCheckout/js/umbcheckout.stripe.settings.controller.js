function UmbCheckout($scope, editorService, umbCheckoutResources, umbCheckoutStripeResources, $routeParams, notificationsService, formHelper) {
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
                    notificationsService.success("Stripe settings saved", "The Stripe settings have been saved successfully");
                    vm.saveButtonState = "success";
                    $scope.configurationForm.$dirty = false;
                })
                .catch(
                    function (response) {
                        notificationsService.error("Stripe settings failed to save", "There was an issue trying to save the Stripe settings");
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