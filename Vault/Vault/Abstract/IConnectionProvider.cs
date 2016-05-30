namespace Vault.Abstract
{
    public interface IConnectionProvider
    {
        string GetServer();
        string GetDatabase();
    }
}
