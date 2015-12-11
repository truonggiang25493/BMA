using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BMA.Business;
using BMA.DBChangesNotifer;
using Microsoft.AspNet.SignalR;
using BMA.Hubs;
using BMA.Models;

namespace BMA
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static ChangeStatusNotifier changeStatusNotifer = new ChangeStatusNotifier();
        public static ChangeNotifier notifier = new ChangeNotifier();
        public static QuantityLowNotifer lowQuantityNotifer = new QuantityLowNotifer();
        public static ChangeToConfirmNotifier changeToConfirmNotifier = new ChangeToConfirmNotifier();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(50);

            notifier.Start("BMAChangeDB", "SELECT OrderId FROM dbo.[Orders]");
            notifier.Change += this.OnChange;

            lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
            lowQuantityNotifer.Change += this.OnChange2;

            System.Timers.Timer timer = new System.Timers.Timer {Interval = 86400000};
            timer.Elapsed += timer_Elapsed;
            timer.Start();
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

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            InputMaterialBusiness business = new InputMaterialBusiness();
            business.CheckInputMaterialListStartup();
        }

    }

}
