using System.Threading.Tasks;

namespace Vault.Abstract
{
    public interface IExistingChecker
    {
        Task<bool> Exist(string dbName);
    }
}
