using ALS.Glance.Models;
using ALS.Glance.Models.Core.Connection;
using ALS.Glance.UoW;
using ALS.Glance.UoW.Core;
using ALS.Glance.UoW.IoC;
using ALS.Glance.UoW.Mapping;
using ALS.Glance.UoW.Security.Context.Implementation;
using ALS.Glance.UoW.Security.UnitOfWork.Interfaces;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Configuration;

namespace ALS.Glance.Api.IoC.Installers
{
    public class UnitOfWorkInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(                
                Component.For<ALSContext, SecurityDbContext>()
                    .UsingFactoryMethod(
                        k => new ALSContext(k.Resolve<IConnectionString>().Value))
                    .LifestyleTransient(),
                Component.For<UnitOfWorkSelector>().LifestyleSingleton(),
                Component.For<IUnitOfWorkFactory>()
                    .AsFactory(x => x.SelectedWith<UnitOfWorkSelector>()).LifestyleSingleton(),

                Component.For<IALSUnitOfWork, ISecurityUnitOfWork>()
                    .ImplementedBy<ALSUnitOfWork>()
                    .Named(typeof (IALSUnitOfWork).Name)
                    .LifestyleScoped());
        }
    }
}