'use strict';
angular.module('trustlyApp', ['ngRoute'])
    .config(['$routeProvider', '$httpProvider', function ($routeProvider, $httpProvider) {

        // disable IE ajax request caching
        if (!$httpProvider.defaults.headers.get) {
            $httpProvider.defaults.headers.get = {};
        }
        $httpProvider.defaults.headers.get['If-Modified-Since'] = '0';

        $routeProvider.when("/Home", {
            controller: "resultListCtrl",
            templateUrl: "/App/Views/ResultList.html",
        }).otherwise({ redirectTo: "/Home" });

    }]);