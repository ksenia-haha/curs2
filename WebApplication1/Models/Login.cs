using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Log
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Введите логин")]
        public string Login { get; set; }


        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Введите пароль")]
        //[DataType(DataType.Password)]
        public string Password { get; set; }
     
    }
}
