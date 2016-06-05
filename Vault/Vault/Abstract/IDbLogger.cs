namespace Vault.Abstract
{
    public interface IDbLogger:ILogger
    {
        string EventType { get; set; }
        string VaultId { get; set; }
    }
}
