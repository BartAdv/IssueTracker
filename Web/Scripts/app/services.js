angular.module("IssueService", ["ngResource"])
    .factory("ReportedIssue", function ($resource) {
        return $resource(
            "/user/:user/issues/reported",
            {
                user: "@user"
            },
            { "report": { method: "POST" } }
       );
    });