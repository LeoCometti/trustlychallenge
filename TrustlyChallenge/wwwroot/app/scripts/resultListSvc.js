'use strict';
angular.module('trustlyApp')
    .factory('resultListSvc', ['$http', function ($http) {

        $http.defaults.useXDomain = true;
        delete $http.defaults.headers.common['X-Requested-With'];

        return {
            getItems: function () {
                return $http.get(apiEndpoint + '/api/GitHubRepository');
            },
            postItem: function (item) {
                return $http.post(apiEndpoint + '/api/GitHubRepository', item);
            },
            putItem: function (item) {
                return $http.put(apiEndpoint + '/api/GitHubRepository/' + item.id, item);
            },
        };
    }]);