﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BMA.DBChangesNotifer;
using Microsoft.AspNet.SignalR;
using BMA.Hubs;

namespace BMA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var notifier = new ChangeNotifier();
            notifier.Start("BMAChangeDB", "SELECT OrderId FROM dbo.[Orders]");
            notifier.Change += this.OnChange;

            var lowQuantityNotifer = new QuantityLowNotifer();
            lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
            lowQuantityNotifer.Change += this.OnChange2;
        }

        protected void Application_BeginRequest()
        {
            if (System.Web.HttpContext.Current.Application["UserId"] != null)
            {
                var changeStatusNotifer = new ChangeStatusNotifier();
                string dependencyCheckSql = string.Format("{0}{1}", "SELECT OrderId FROM dbo.[Orders] WHERE CustomerUserId=", Convert.ToInt32(System.Web.HttpContext.Current.Application["UserId"]));
                changeStatusNotifer.Start("BMAChangeDB", dependencyCheckSql);
                changeStatusNotifer.Change += this.OnChange3;
            }
        }

        private void OnChange(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange(e.Info, e.Source, e.Type);
        }

        private void OnChange2(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange2(e.Info, e.Source, e.Type);
        }

        private void OnChange3(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange3(e.Info, e.Source, e.Type);
        }
    }
}
