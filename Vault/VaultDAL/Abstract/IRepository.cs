using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaultDAL.Abstract
{
    public interface IRepository<T>:IDisposable where T : class
    {
        Task<IEnumerable<T>> GetListAsync();
        Task<T> GetItemAsync(string id);
        Task<T> CreateAsync(T item);
        Task DeleteAsync(string id);
        Task UpdateAsync(T item);
    }
}
