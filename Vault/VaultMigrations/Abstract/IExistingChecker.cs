using System.Threading.Tasks;

namespace VaultMigrations.Abstract
{
    public interface IExistingChecker
    {
        Task<bool> Exist(string dbName);
    }
}
