function IssueCtrl($scope, $routeParams, ReportedIssue) {
    $scope.newIssue = new ReportedIssue({ user: $routeParams.user });
    $scope.report = function (issue) {
        issue.$report();
    };
}