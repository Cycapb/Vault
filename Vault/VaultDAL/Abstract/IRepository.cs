using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaultDAL.Abstract
{
    public interface IRepository<T>:IDisposable where T : class
    {
        IEnumerable<T> GetList();
        Task<IEnumerable<T>> GetListAsync();
        T GetItem(string id);
        Task<T> GetItemAsync(string id);
        Task<T> CreateAsync(T item);
        Task DeleteAsync(string id);
        Task UpdateAsync(T item);
    }
}
