using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.Models
{
    class EmployeeModel : UserModel
    {
        [Key]
        public int EmployeeId { get; set; }
        public virtual List<RepairOrderModel> Orders {get; set;}
        public double Fee { get; set; }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(FirstName))
            {
                return FirstName;
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
