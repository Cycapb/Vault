using System.Configuration;

namespace VaultService.Models.Mail
{
    public class EmailSettings
    {

        public string MailTo { get; set; }
        public string MailFrom { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string ServerName { get; private set; }
        public int ServerPort { get; private set; }
        public bool UseSsl { get; private set; }

        public EmailSettings()
        {
            ApplySettings();
        }


        private void ApplySettings()
        {
            this.MailFrom = ConfigurationManager.AppSettings["MailFrom"];
            this.UserName = ConfigurationManager.AppSettings["UserName"];
            this.Password = ConfigurationManager.AppSettings["Password"];
            this.ServerName = ConfigurationManager.AppSettings["Server"];
            this.ServerPort = int.Parse(ConfigurationManager.AppSettings["Port"]);
            this.UseSsl = bool.Parse(ConfigurationManager.AppSettings["UseSsl"]);
        }
    }

}