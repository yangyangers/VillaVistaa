using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace VillaVista.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Implement actual email sending logic (SMTP, SendGrid, etc.)
            return Task.CompletedTask;
        }
    }
}
