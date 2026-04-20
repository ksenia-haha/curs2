using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Sale
    {
        public int? Id { get; set; }
        public int ClientId { get; set; }
        public int EmployeeId { get; set; }
        public Client Client {  get; set; }
        public Employee Employee { get; set; }
        public string Date { get; set; }
        public int? Sum { get; set; }

        public List<Return> Returns { get; set; }
        public List<SaleAndExemplar> salesAndExemplars { get; set; }

    }
}
