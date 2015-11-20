using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BMA.Startup))]
namespace BMA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
