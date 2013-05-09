fromCategory("Issue")
.foreachStream()
.when({
  Reported: function (s, e) {
    linkTo("IssuesBy-" + e.body.value.Reporter, e);
  },
  Taken: function (s, e) {
    linkTo("IssuesBy-" + e.body.value.User, e);
  }
});