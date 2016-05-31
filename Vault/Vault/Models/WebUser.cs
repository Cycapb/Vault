using Vault.Abstract;
using VaultDAL.Concrete;

namespace Vault.Models
{
    public class WebUser:Entity,IWebUser
    {
        public string UserName { get; set; }
    }
}