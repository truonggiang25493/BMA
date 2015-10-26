using System.Web;
using System.Web.Mvc;
using jsreport.Embedded;
using jsreport.MVC;

namespace BMA
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            //when using .net embedded version you need to start server first
            EmbeddedReportingServer embeddedReportingServer = new EmbeddedReportingServer();
            embeddedReportingServer.RelativePathToServer = "../App_Data";
            embeddedReportingServer.StartAsync().Wait();
            filters.Add(new JsReportFilterAttribute(embeddedReportingServer.ReportingService));
        }
    }
}
