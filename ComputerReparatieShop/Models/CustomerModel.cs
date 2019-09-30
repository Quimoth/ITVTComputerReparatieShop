using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.Models
{
    public class CustomerModel : UserModel, IComparable
    {
        [Key]
        public int CustomerId { get; set; }
        public virtual List<RepairOrderModel> RepairOrders { get; set; }
        public int CompareTo(object obj)
        {
            CustomerModel customer = obj as CustomerModel;
            return string.Compare(this.FirstName, customer.FirstName);
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
