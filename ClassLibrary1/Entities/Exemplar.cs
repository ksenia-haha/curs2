using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain
{
    public class Exemplar
    {
        public int Id { get; set; }
        public string EditionISBN { get; set; }
        public Edition? Edition { get; set; }
        public string Section { get; set; }
        public string Shelf { get; set; }
        public int Price { get; set; } 
        public ExemplarStatus Status { get; set; } = ExemplarStatus.InStock;

        public enum ExemplarStatus
        {
            [Display(Name = "В наличии")]
            InStock = 1,

            [Display(Name = "Продан")]
            Sold = 2,

            [Display(Name = "Брак")]
            Defective = 3
        }

        public List<SaleAndExemplar> SaleAndExemplars;
        public List<Return> Returns { get; set; }
    }
}
