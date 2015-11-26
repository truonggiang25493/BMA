using System;
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
        public static ChangeStatusNotifier changeStatusNotifer = new ChangeStatusNotifier();
        public static ChangeNotifier notifier = new ChangeNotifier();
        public static QuantityLowNotifer lowQuantityNotifer = new QuantityLowNotifer();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            notifier.Start("BMAChangeDB", "SELECT OrderId FROM dbo.[Orders]");
            notifier.Change += this.OnChange;

            lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
            lowQuantityNotifer.Change += this.OnChange2;
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
    }
    
}
