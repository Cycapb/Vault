using System.Threading.Tasks;

namespace VaultServices.Abstract
{
    public interface ILogger
    {
        Task Log(string message);
    }
}
