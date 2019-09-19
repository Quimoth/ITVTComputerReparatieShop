using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.Models
{
    class RepairOrderModel
    {
        [Key]
        public int RepairOrderId { get; set;}
        public string Status { get; set; }
        public CustomerModel Customer { get; set; }
        public DateTime? StartDate { get; set; }
        public EmployeeModel Employee { get; set; }
        public string Description { get; set; }
        public DateTime? EndDate { get; set; }
        public double HoursWorked { get; set; }
        public virtual List<PartModel> PartsUsed { get; set; }

    }
}
