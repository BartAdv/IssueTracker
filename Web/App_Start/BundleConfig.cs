using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace IssueTracker.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/angular")
                .Include("~/Scripts/angular/angular.js",
                  "~/Scripts/angular/angular-resource.js"));

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/Scripts/app/services.js",
                    "~/Scripts/app/controllers.js",
                    "~/Scripts/app/app.js"));
        }
    }
}