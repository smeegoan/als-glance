using System;
using ALS.Glance.Api.Properties;
using ALS.Glance.Api.Security;
using ALS.Glance.Models.Security.Implementations;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.DataProtection;

namespace ALS.Glance.Api.IoC.Installers
{
    public class SecurityInstaller : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer"/>.
        /// </summary>
        /// <param name="container">The container.</param><param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IDataProtectionProvider>()
                    .ImplementedBy<DpapiDataProtectionProvider>()
                    .LifestyleSingleton(),
                Component.For<IUserTokenProvider<ApiUser, string>>()
                    .ImplementedBy<DataProtectorTokenProvider<ApiUser>>().LifestyleSingleton()
                    .UsingFactoryMethod(
                        k => new DataProtectorTokenProvider<ApiUser>(
                            k.Resolve<IDataProtectionProvider>().Create("EmailConfirmation"))
                        {
                            TokenLifespan = TimeSpan.FromHours(1)
                        }),
                Component.For<IIdentityMessageService>()
                    .ImplementedBy<SmtpEmailService>().LifestyleSingleton()
                    .DynamicParameters(
                        (k, d) =>
                        {
                            d["from"] = k.Resolve<Settings>().EmailFrom;
                        }));
        }
    }
}