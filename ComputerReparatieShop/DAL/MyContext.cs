using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.DAL
{
    public class MyContext : DbContext
    {
        [NotMapped]
        private static MyContext myContext;
        private MyContext()
        {
            
        }
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<PartModel> Parts { get; set; }
        public DbSet<RepairOrderModel> Repairs { get; set; }

        public static MyContext Create()
        {
            if (myContext == null)
            {
                myContext = new MyContext();
            }
            return myContext;
        }
    }
}
