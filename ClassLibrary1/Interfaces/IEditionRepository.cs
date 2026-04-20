
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEditionRepository : IRepository<Edition>
    {
        Task<IEnumerable<Edition>> GetByTitle(Edition item);
        Task<IEnumerable<Edition>> GetByAuthor(Edition item);
    }
}

