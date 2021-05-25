using System;
using System.Net.Mail;
using System.Threading.Tasks;
using WebApp.Settings;
using Microsoft.Extensions.Options;
using WebApp.Models;

namespace WebApp.Services
{
    public class BasicSMTPService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public BasicSMTPService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_mailSettings.Mail);
            mailMessage.To.Add(new MailAddress(mailRequest.ToEmail));

            mailMessage.Subject = mailRequest.Subject;
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = mailRequest.Body;

            SmtpClient client = new SmtpClient();
            client.Credentials = new System.Net.NetworkCredential(_mailSettings.Mail, _mailSettings.Password);
            client.Host = _mailSettings.Host;
            client.Port = _mailSettings.Port;
            client.EnableSsl = true;
            try
            {
                await client.SendMailAsync(mailMessage);
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}