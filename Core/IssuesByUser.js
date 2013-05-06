fromStream("ReportedIssues")
.partitionBy(function (e) { return e.body.value.Reporter; })
.when({
  $init: function () {
    return [];
  },

  Reported: function (s, e) {
    var id = e.streamId.replace("Issue-", "");
    s.push(id);
    return s;
  },
});
