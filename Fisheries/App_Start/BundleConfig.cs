﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Optimization;

namespace Fisheries
{
    public class BundleConfig
    {
        // 有关绑定的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
                "~/Scripts/knockout-{version}.js",
                "~/Scripts/knockout.validation.js"));

            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                "~/Scripts/sammy-{version}.js",
                "~/Scripts/app/common.js",
                "~/Scripts/app/app.datamodel.js",
                "~/Scripts/app/app.viewmodel.js",
                "~/Scripts/app/home.viewmodel.js",
                "~/Scripts/app/_run.js"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备时，请使用 http://modernizr.com 上的生成工具来仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-datepicker.js",
                "~/Scripts/bootstrap-datepicker.min.js",
                "~/Scripts/bootstrap-datetimepicker.js",
                "~/Scripts/bootstrap-datetimepicker.min.js",
                "~/Scripts/moment-with-langs.js",
                "~/Scripts/moment-with-langs.min.js",
                "~/Scripts/moment.js",
                "~/Scripts/moment.min.js",
                "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                 "~/Content/bootstrap.css",
                 "~/Content/Site.css"));

            bundles.Add(new ScriptBundle("~/bundles/avatar").Include(
                     "~/Scripts/site.avatar.js"));
            bundles.Add(new ScriptBundle("~/bundles/events").Include(
                    "~/Scripts/site.events.js"));

            bundles.Add(new ScriptBundle("~/bundles/jcrop").Include(
                      "~/Scripts/jquery.Jcrop.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryform").Include(
                      "~/Scripts/jquery.form.js"));

            bundles.Add(new StyleBundle("~/Content/jcrop").Include(
                      "~/Content/jquery.Jcrop.css"));

            bundles.Add(new StyleBundle("~/Content/event").Include(
                      "~/Content/site.avatar.event.css"));

            bundles.Add(new StyleBundle("~/Content/ad").Include(
                     "~/Content/site.avatar.ad.css"));

            bundles.Add(new StyleBundle("~/Content/homeAd").Include(
                     "~/Content/site.avatar.home.ad.css"));

            bundles.Add(new StyleBundle("~/Content/info").Include(
                     "~/Content/site.avatar.information.css"));

            bundles.Add(new StyleBundle("~/Content/cele").Include(
                     "~/Content/site.avatar.event.css"));
        }
    }
}
