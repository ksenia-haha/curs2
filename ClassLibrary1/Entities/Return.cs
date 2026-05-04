using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{
    public class Return
    {
        public int? Id { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int ExemplarId { get; set; }

        public ReturnStatus Status { get; set; } = ReturnStatus.InProcess;

        public enum ReturnStatus
        {
            [Display(Name = "В обработке")]
            InProcess = 1,

            [Display(Name = "Принято")]
            Yes = 2,

            [Display(Name = "Отказано")]
            No = 3
        }

        public Employee Employee { get; set; }
        public Client Client { get; set; }
        public Exemplar Exemplar { get; set; }
    }
}
