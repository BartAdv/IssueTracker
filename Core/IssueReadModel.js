fromCategory("Issue")
.foreachStream()
.when({
  $init: function () {
    return {
      Number: 0,
      Reporter: "",
      Status: "",
      Summary: "",
    };
  },

  Reported: function (s, e) {
    s.Number = e.body.value.Number;
    s.Status = "Logged";
    s.Reporter = e.body.value.Reporter;
    s.Summary = e.body.value.Summary;
    return s;
  },

  Closed: function (s, e) {
    s.Status = "Closed";
    return s;
  },

  Cancelled: function (s, e) {
    s.Status = "Cancelled";
    return s;
  },

});

