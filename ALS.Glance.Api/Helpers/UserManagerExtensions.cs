using System.Threading;
using System.Threading.Tasks;
using ALS.Glance.Api.Properties;
using ALS.Glance.Models.Security.Implementations;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Helpers
{
    internal static class UserManagerExtensions
    {

        public static async Task SendEmailConfirmationAsync<TApiUser>(
            this UserManager<TApiUser> userManager, string userId, string emailConfirmationUrl, CancellationToken ct)
            where TApiUser : ApiUser
        {
            ct.ThrowIfCancellationRequested();
            await userManager.SendEmailAsync(
                userId,
                Resources.EmailSubjectConfirmation,
                string.Format(Resources.EmailBodyConfirmation, emailConfirmationUrl));
        }

        public static async Task SendEmailPasswordResetConfirmationAsync<TApiUser>(
            this UserManager<TApiUser> userManager, string userId, string emailConfirmationUrl, CancellationToken cancellationToken = default(CancellationToken))
            where TApiUser : ApiUser
        {
            cancellationToken.ThrowIfCancellationRequested();
            await userManager.SendEmailAsync(
                userId,
                Resources.EmailSubjectPasswordResetConfirmation,
                string.Format(Resources.EmailBodyPasswordResetConfirmation, emailConfirmationUrl));
        }

     
    }
}