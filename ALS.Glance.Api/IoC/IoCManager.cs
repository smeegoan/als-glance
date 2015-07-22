using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using ALS.Glance.Api.IoC.Installers;

namespace ALS.Glance.Api.IoC
{
    public static class IoCManager
    {
        public static void RegisterDependenciesInto(this IWindsorContainer container)
        {
            container.Kernel
                .AddFacility<TypedFactoryFacility>()
                .Resolver
                .AddSubResolver(
                    new ArrayResolver(container.Kernel, true));

            container.Install(
                       new ControllerInstaller(),
                       new ConfigurationInstaller(),
                       new UnitOfWorkInstaller());
        }

        public static IWindsorContainer NewContainer()
        {
            var container = new WindsorContainer();

            container.RegisterDependenciesInto();

            return container;
        }
    }
}