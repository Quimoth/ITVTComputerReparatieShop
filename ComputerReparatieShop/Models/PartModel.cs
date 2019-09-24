using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.Models
{
    public class PartModel
    {
        [Key]
        public int PartId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int InStock { get; set; }
        public int Ordered { get; set; }
        public DateTime? NextDelivery { get; set; }
        public string Manufacturer { get; set; }
        public virtual List<RepairOrderModel> RepairOrders { get; set; }
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Name))
            {
                return Name;
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
