using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Client
    {
        public int? Id {  get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string? Patronymic { get; set; }
        public string PhoneNumber { get; set; }

        public List<Sale> Sales { get; set; }
        public List<Return> Returns { get; set; }

    }
}
