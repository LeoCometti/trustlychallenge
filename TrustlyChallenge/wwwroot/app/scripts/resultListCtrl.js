'use strict';
angular.module('trustlyApp')
    .controller('resultListCtrl', ['$scope', '$location', 'resultListSvc', function ($scope, $location, resultListSvc) {
        $scope.error = "";
        $scope.loadingMessage = "Loading...";
        $scope.resultList = null;
        $scope.editingInProgress = false;
        $scope.newUrlAddress = "";

        $scope.populate = function () {
            resultListSvc.getItems().success(function (results) {
                $scope.resultList = results;
                $scope.loadingMessage = "";
            }).error(function (err) {
                $scope.error = err;
                $scope.loadingMessage = "";
            })
        };
        $scope.update = function (todo) {
            $scope.editInProgressTodo.isComplete = todo.isComplete;
            resultListSvc.putItem($scope.editInProgressTodo).success(function (results) {
                $scope.loadingMsg = "";
                $scope.populate();
                $scope.editSwitch(todo);
            }).error(function (err) {
                $scope.error = err;
                $scope.loadingMessage = "";
            })
        };
        $scope.add = function () {
            if ($scope.editingInProgress) {
                $scope.editingInProgress = false;
            }
            resultListSvc.postItem({
                'Url': $scope.newUrlAddress
            }).success(function (results) {
                $scope.loadingMsg = "";
                $scope.newUrlAddress = "";
                $scope.populate();
            }).error(function (err) {
                $scope.error = err;
                $scope.loadingMsg = "";
            })
        };

    }]);
