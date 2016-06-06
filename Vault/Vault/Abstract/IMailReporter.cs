using System.Threading.Tasks;

namespace Vault.Abstract
{
    public interface IMailReporter
    {
        Task Report(string message);
        string MailTo { get; set; }
    }
}
