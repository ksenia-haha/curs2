using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Edition
    {
        [Display(Name = "ISBN")]
        [Required(ErrorMessage ="Поле должно быть заполнено")]
        [Key]
        public string ISBN { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Name { get; set; }

        [Display(Name = "Автор")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Author { get; set; }

        [Display(Name = "Жанр")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Genre { get; set; }

        [Display(Name = "Издательство")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Publisher { get; set; }

        [Display(Name = "Год выпуска")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        [Range(1, 2026, ErrorMessage ="Некорректная дата")]
        public int Year { get; set; }

        //[Display(Name = "Количество в магазине")]
        //[Required(ErrorMessage = "Поле должно быть заполнено")]
        //[Range(1, int.MaxValue, ErrorMessage = "Некорректное число")]
        //public int CountShop { get; set; }

        //[Display(Name = "Количество на складе")]
        //[Required(ErrorMessage = "Поле должно быть заполнено")]
        //[Range(1, int.MaxValue, ErrorMessage = "Некорректное число")]
        //public int CountSklad { get; set; }
    }
}
