using System.Collections.Generic;

namespace Vault.Abstract
{
    public interface IUserGetter<out T> where T : class
    {
        IEnumerable<T> Get();
    }
}
