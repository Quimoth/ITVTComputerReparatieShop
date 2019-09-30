using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.Models
{
    public class EmployeeModel : UserModel, IComparable
    {
        [Key]
        public int EmployeeId { get; set; }
        public virtual List<RepairOrderModel> Orders {get; set;}
        public double Fee { get; set; }
        public int CompareTo(object obj)
        {
            EmployeeModel employee = obj as EmployeeModel;
            return string.Compare(this.FirstName, employee.FirstName);
        }
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
