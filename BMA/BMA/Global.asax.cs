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
        public static CancelOrderNotifier cancelOrderNotifier = new CancelOrderNotifier();
        public static ConfirmToCustomerNotifier confirmToCustomerNotifer = new ConfirmToCustomerNotifier();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(50);


            System.Timers.Timer timer = new System.Timers.Timer {Interval = 86400000};
            timer.Elapsed += timer_Elapsed;
            timer.Start();
        }

        private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            InputMaterialBusiness business = new InputMaterialBusiness();
            business.CheckInputMaterialListStartup();
            // Auto remove order if delivery date < Today
            OrderBusiness orderBusiness = new OrderBusiness();
            orderBusiness.AutoRemoveWaitingOrder();
        }

    }

}
