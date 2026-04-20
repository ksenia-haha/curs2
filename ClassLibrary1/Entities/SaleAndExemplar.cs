using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class SaleAndExemplar
    {
        public int SaleId { get; set; }
        public int ExemplarId { get; set; }

        public Sale Sale { get; set; }
        public Exemplar Exemplar { get; set; }
    }
}
