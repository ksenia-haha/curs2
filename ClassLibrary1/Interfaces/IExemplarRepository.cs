
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IExemplarRepository : IRepository<Exemplar>
    {
        Task<IEnumerable<Exemplar>> GetByEditionId(string id);
    }
}
