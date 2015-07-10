using System.Web.Http.Dispatcher;
using Castle.Windsor;
using ALS.Glance.Api.IoC;
using ALS.Glance.Api.IoC.Factories;
using System.Web.Http;

namespace ALS.Glance.Api
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        private volatile IWindsorContainer _container;

        protected void Application_Start()
        {
            _container = IoCManager.NewContainer();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.Filters.RegisterGlobalFilters();
            GlobalConfiguration.Configuration.Services.Replace(
                typeof(IHttpControllerActivator),
                new WindsorApiControllerFactory(_container));
        }

        protected void Application_End()
        {
            var container = _container;
            if (container == null) return;

            _container = null;
            container.Dispose();
        }
    }
}
