using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ALS.Glance.Api.Helpers.Binder;
using ALS.Glance.Api.Properties;
using ALS.Glance.Models.Core.Connection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ALS.Glance.Api.IoC.Installers
{
    public class ConfigurationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<Settings>()
                    .UsingFactoryMethod(() => Settings.Default)
                    .LifestyleSingleton(),
                Component.For<IODataQueryOptionsBinder>()
                    .ImplementedBy<ODataQueryOptionsBinder>().LifestyleSingleton(),
                Component
                    .For<IConnectionString>()
                    .UsingFactoryMethod(
                        () => (ConnectionString)
                            ConfigurationManager.ConnectionStrings["ALSContext"].ConnectionString)
                    .LifestyleSingleton());
        }
    }
}