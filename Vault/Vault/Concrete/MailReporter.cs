using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Vault.Abstract;
using Vault.Infrastructure.Filters;
using Vault.Models;

namespace Vault.Concrete
{
    [MailError]
    public class MailReporter:IMailReporter
    {
        private readonly EmailSettings _emailSettings;

        public string MailTo { get; set; }

        public MailReporter(EmailSettings emailSettings)
        {
            _emailSettings = emailSettings;
        }

        public async Task Report(string message)
        {
            if (this.MailTo == null)
            {
                return;
            }
             await SendReport(message);
        }

        private async Task SendReport(string message)
        {
            var smtpClient = CreateSmtpClient();
            _emailSettings.MailTo = this.MailTo;

            StringBuilder messageBody = new StringBuilder();
            messageBody.Append(message);

            MailMessage mailMessage = new MailMessage(_emailSettings.MailFrom,_emailSettings.MailTo,"Unauthorized access to the vault",messageBody.ToString());

            await smtpClient.SendMailAsync(mailMessage);
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient()
            {
                Host = _emailSettings.ServerName,
            Port = _emailSettings.ServerPort,
            EnableSsl = _emailSettings.UseSsl,
            Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password)
        };

                return client;
            
        }
    }
}