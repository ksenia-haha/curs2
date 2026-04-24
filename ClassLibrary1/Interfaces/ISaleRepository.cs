using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISaleRepository : IRepository<Sale>
    {
        Task<IEnumerable<Sale>> GetByClientId(int id);
        Task<IEnumerable<Sale>> GetByEmployeeId(int id);
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}
