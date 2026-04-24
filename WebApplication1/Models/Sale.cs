using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Sale
    {
        [Display(Name = "ID")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int? Id { get; set; }

        [Display(Name = "Клиент")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int ClientId { get; set; }


        [Display(Name = "Сотрудник")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int EmployeeId { get; set; }


        [Display(Name = "Дата")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Date { get; set; }

        [Display(Name = "Сумма")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int? Sum { get; set; }

        public List<SaleAndExemplar> SaleAndExemplars { get; set; }
        public List<Return> Returns { get; set; }
    }
}
