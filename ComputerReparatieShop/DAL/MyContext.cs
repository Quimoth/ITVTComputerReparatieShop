using ComputerReparatieShop.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerReparatieShop.DAL
{
    class MyContext : DbContext
    {
        public DbSet<CustomerModel> Customers { get; set; }
        public DbSet<EmployeeModel> Employees { get; set; }
        public DbSet<PartModel> Parts { get; set; }
        public DbSet<RepairOrderModel> Repairs { get; set; }
    }
}
