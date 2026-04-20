
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task CreateAsync(T item);
        Task DeleteAsync(T item);
        Task UpdateAsync(T item);
        Task<T> GetByIdAsync(T item);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
