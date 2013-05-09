angular.module('IssueTracker', [])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/index', { templateUrl: 'Partials/Issues.html', controller: IssuesCtrl })
        .otherwise({ redirectTo: '/index' });
}]);