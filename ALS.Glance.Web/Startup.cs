using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ALS.Glance.Web.Startup))]
namespace ALS.Glance.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
