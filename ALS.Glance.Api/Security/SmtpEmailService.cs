using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;

namespace ALS.Glance.Api.Security
{
    public class SmtpEmailService : IIdentityMessageService
    {
        private readonly string _from;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="from"></param>
        public SmtpEmailService(string from)
        {
            _from = from;
        }

        public async Task SendAsync(IdentityMessage message)
        {
            using (var smtpClient = new SmtpClient())
            using (
                var mailMessage =
                    new MailMessage(_from, message.Destination, message.Subject, message.Body)
                    {
                        IsBodyHtml = true,
                    })
            {
                mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.Never;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}