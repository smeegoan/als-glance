using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using ALS.Glance.Models;
using ALS.Glance.Models.Core.Connection;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using ALS.Glance.UoW.IoC;
using ALS.Glance.UoW.Security.Context.Implementation;
using ALS.Glance.UoW.Security.Context.Interfaces;
using ALS.Glance.UoW.Security.UnitOfWork.Interfaces;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace ALS.Glance.Web.Ioc.Installers
{
    public class UnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component
                    .For<IConnectionString>()
                    .UsingFactoryMethod(
                        () => (ConnectionString)
                            ConfigurationManager.ConnectionStrings["ALSContext"].ConnectionString)
                    .LifestyleSingleton(),
                Component.For<SecurityDbContext>()
                    .UsingFactoryMethod(
                        k => new SecurityDbContext(k.Resolve<IConnectionString>().Value))
                    .LifestyleTransient(),
                Component.For<UnitOfWorkSelector>().LifestyleSingleton(),
                Component.For<IUnitOfWorkFactory>()
                    .AsFactory(x => x.SelectedWith<UnitOfWorkSelector>()).LifestyleSingleton(),

                Component.For<ISecurityDbContext, ISecurityUnitOfWork>()
                    .ImplementedBy<SecurityDbContext>()
                    .Named(typeof(ISecurityDbContext).Name)
                    .LifestyleScoped());
        }
    }
}