using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PopupDemo.Startup))]
namespace PopupDemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
