using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using VaultService.Models.Mail;

namespace VaultService.Models.Mail
{

    public class MailReporter
    {
        private readonly EmailSettings _emailSettings;
        

        public string MailTo { get; set; }

        public MailReporter()
        {
            _emailSettings = new EmailSettings();
        }

        public async Task Report(string fileName)
        {
            if (this.MailTo == null)
            {
                return;
            }
            try
            {
                await SendReport(fileName);
            }
            catch (Exception)
            {
                return;
            }
        }

        private async Task SendReport(string fileName)
        {
            var smtpClient = CreateSmtpClient();
            _emailSettings.MailTo = this.MailTo;

            StringBuilder messageBody = new StringBuilder();
            messageBody.Append("Log is in attache");

            MailMessage mailMessage = new MailMessage(_emailSettings.MailFrom, _emailSettings.MailTo,
                "Daily access report", messageBody.ToString());
            mailMessage.Attachments.Add(new Attachment(fileName));

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