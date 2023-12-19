angular.module("umbraco.resources").factory("umbCheckoutStripeResources", function ($http) {
    return {
        getShippingRates: function () {

            return $http.get("backoffice/UmbCheckout/StripeShippingRatesApi/GetShippingRates")
                .then(function (response) {
                    return response;
                }
                );
        },
        getStripeShippingRates: function () {

            return $http.get("backoffice/UmbCheckout/StripeShippingRatesApi/GetStripeShippingRates")
                .then(function (response) {
                    return response;
                }
                );
        },
        getShippingRate: function (key) {

            return $http.get("backoffice/UmbCheckout/StripeShippingRatesApi/GetShippingRate?key=" + key)
                .then(function (response) {
                    return response;
                }
                );
        },
        getStripeShippingRate: function (id) {

            return $http.get("backoffice/UmbCheckout/StripeShippingRatesApi/GetStripeShippingRate?id=" + id)
                .then(function (response) {
                    return response;
                }
                );
        },
        createShippingRate: function (configurationValues) {
            return $http.put("backoffice/UmbCheckout/StripeShippingRatesApi/CreateShippingRate", configurationValues)
                .then(function (response) {
                    return response;
                }
                );
        },
        updateShippingRate: function (configurationValues, key) {
            configurationValues.key = key
            return $http.patch("backoffice/UmbCheckout/StripeShippingRatesApi/UpdateShippingRate", configurationValues)
                .then(function (response) {
                    return response;
                }
                );
        },
        deleteShippingRate: function (key) {

            return $http.delete("backoffice/UmbCheckout/StripeShippingRatesApi/DeleteShippingRate?key=" + key)
                .then(function (response) {
                    return response;
                }
                );
        },
        getStripeSettings: function () {

            return $http.get("backoffice/UmbCheckout/StripeSettingsApi/GetStripeSettings")
                .then(function (response) {
                    return response;
                }
                );
        },
        updateStripeSettings: function (configurationValues) {

            return $http.patch("backoffice/UmbCheckout/StripeSettingsApi/UpdateStripeSettings", configurationValues)
                .then(function (response) {
                    return response;
                }
                );
        }
    }
});