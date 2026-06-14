
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExemplarRepository : IRepository<Exemplar>
    {
        Task<IEnumerable<Exemplar>> GetAvailableExemplarsAsync();
        Task<IEnumerable<Exemplar>> GetByEditionId(string id);
        Task<Exemplar> GetByIntIdAsync(int exemplarId);
        Task UpdateStatusSoldAsynс(int exemplarId);
        Task<IEnumerable<Exemplar>> GetSoldExemplarsAsync();
    }
}
