namespace Vault.Abstract
{
    public interface IAccessManager
    {
        void GrantReadAccess(string userId);
        void GrantCreateAccess(string userId);
    }
}
