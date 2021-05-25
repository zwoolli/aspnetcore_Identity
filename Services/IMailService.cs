using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}