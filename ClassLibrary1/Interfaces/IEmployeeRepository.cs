
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface IEmployeeRepository : IRepository<Employee>
    {
        Task<IEnumerable<Employee>> GetByPosition(Employee item);
        Task<IEnumerable<Employee>> SearchBySurname(Employee item);
        Task<Employee> Login(Employee item);
        
    }
}
