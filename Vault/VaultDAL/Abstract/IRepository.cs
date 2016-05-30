﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace VaultDAL.Abstract
{
    public interface IMongoRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetListAsync();
        Task<T> GetItemAsync(string id);
        Task<T> CreateAsync(T item);
        Task DeleteAsync(string id);
        Task UpdateAsync(T item);
    }
}
