namespace VaultDAL.Abstract
{
    public interface IConnectionProvider
    {
        string GetServer();
        string GetDatabase();
    }
}
