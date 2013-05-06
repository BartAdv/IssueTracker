fromCategory("Issue")
.foreachStream()
.when({
  Reported: function (s, e) {
    linkTo("ReportedIssues", e);
  },
});
