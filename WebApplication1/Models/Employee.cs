using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Employee
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int? Id { get; set; }

        [Display(Name = "Фамилия")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Surname { get; set; }

        [Display(Name = "Имя")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Name { get; set; }

        [Display(Name = "Отчество")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Patronymic { get; set; }

        [Display(Name = "Должность")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Position { get; set; }

        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Password { get; set; }

        [Display(Name = "Уровень доступа")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int Level { get; set; }
    }
}
