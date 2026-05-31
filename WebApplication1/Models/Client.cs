using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Client
    {
        [Display(Name = "ID")]
        public int? Id { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string? Patronymic { get; set; }

        [Display(Name = "Номер телефона")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string PhoneNumber { get; set; }

        public Sale Sale { get; set; }
    }
}
