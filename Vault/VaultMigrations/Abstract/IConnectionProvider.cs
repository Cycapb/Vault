namespace VaultMigrations.Abstract
{
    public interface IConnectionProvider
    {
        string GetServer();
        string GetDatabase();
    }
}
