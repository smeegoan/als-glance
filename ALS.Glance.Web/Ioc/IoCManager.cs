using ALS.Glance.Web.Ioc.Installers;
using Castle.Windsor;

namespace ALS.Glance.Web.Ioc
{
    public static class IoCManager
    {
        public static void RegisterDependenciesInto(IWindsorContainer container)
        {
            container.Install(new ControllerInstaller(),
                            new UnitOfWorkInstaller(),
                            new BusinessInstaller());
        }

        public static IWindsorContainer NewContainer()
        {
            var container = new WindsorContainer();

            RegisterDependenciesInto(container);

            return container;
        }
    }
}