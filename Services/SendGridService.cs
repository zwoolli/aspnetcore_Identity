using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public class SendGridService : IMailService
    {
        public SendGridService(IOptions<AuthMessageSenderOptions> optionsAccessor)
        {
            Options = optionsAccessor.Value;
        }

        public AuthMessageSenderOptions Options { get; }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var client = new SendGridClient(Options.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(Options.SendGridEmail, Options.SendGridUser),
                Subject = mailRequest.Subject,
                HtmlContent = mailRequest.Body
            };

            msg.AddTo(new EmailAddress(mailRequest.ToEmail));

            msg.SetClickTracking(false, false);
            
            await client.SendEmailAsync(msg);
        }
    }
}