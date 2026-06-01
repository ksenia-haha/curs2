using System.ComponentModel.DataAnnotations;
using static Domain.Entities.Return;
using static Domain.Exemplar;

namespace WebApplication1.Models
{
    public class Return
    {
        [Display(Name = "ID")]
        public int Id { get; set; }

        [Display(Name = "ID сотрудника")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int EmployeeId { get; set; }

        [Display(Name = "ID клиента")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int ClientId { get; set; }

        [Display(Name = "ID экземпляра")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public int ExemplarId { get; set; }

        [Display(Name = "Причина")]
        [Required(ErrorMessage = "Поле должно быть заполнено")]
        public string Reason { get; set; }

        public ReturnStatus Status { get; set; } = ReturnStatus.InProcess;
    }
}
