using System.Threading.Tasks;

namespace Vault.Abstract
{
    public interface ILogger
    {
        Task Log(string message);
    }
}
