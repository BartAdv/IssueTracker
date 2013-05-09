fromCategory("Issue")
.foreachStream()
.when({
  Reported: function (s, e) {
    linkTo("IssuesBy-" + e.body.Reporter, e);
  },
  Taken: function (s, e) {
    var user = e.body[0];
    linkTo("IssuesBy-" + user, e);
  }
});