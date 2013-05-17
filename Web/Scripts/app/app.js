angular.module('IssueTracker', ["IssueService"])
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider
        .when('/user/:user', { templateUrl: 'Partials/Issues.html', controller: IssueCtrl })
        .otherwise({ redirectTo: '/user/testuser' });
}]);