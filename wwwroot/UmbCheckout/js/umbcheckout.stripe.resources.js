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
        getShippingRate: function (id) {

            return $http.get("backoffice/UmbCheckout/StripeShippingRatesApi/GetShippingRate?id=" + id)
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
        updateShippingRate: function (configurationValues, id) {
            configurationValues.id = id
            return $http.patch("backoffice/UmbCheckout/StripeShippingRatesApi/UpdateShippingRate", configurationValues)
                .then(function (response) {
                    return response;
                }
                );
        },
        deleteShippingRate: function (id) {

            return $http.delete("backoffice/UmbCheckout/StripeShippingRatesApi/DeleteShippingRate?id=" + id)
                .then(function (response) {
                    return response;
                }
                );
        },
    }
});