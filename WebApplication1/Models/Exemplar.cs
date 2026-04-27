using System.ComponentModel.DataAnnotations;
using static Domain.Exemplar;

namespace WebApplication1.Models
{
    public class Exemplar
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int Id { get; set; }

        [Display(Name = "ISBN")]
        public string EditionISBN{ get; set; }

        [Display(Name = "Издание")]
        public Edition? Edition { get; set; }


        [Display(Name = "Секция")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Section { get; set; }

        [Display(Name = "Стеллаж")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Shelf { get; set; }

        [Display(Name = "Цена")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public double Price { get; set; }

        public ExemplarStatus Status { get; set; }

    }
}
