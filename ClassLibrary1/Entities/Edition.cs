using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class Edition
    {
        [Key]
        public string ISBN { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Genre { get; set; }
        public string Publisher { get; set; }
        public int Year { get; set; }
        //public int CountShop { get; set; }
        //public int CountSklad { get; set; }

        public List<Exemplar> Exemplars { get; set; } 
    }
}
