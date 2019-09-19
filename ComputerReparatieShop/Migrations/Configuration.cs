namespace ComputerReparatieShop.Migrations
{
    using ComputerReparatieShop.DAL;
    using ComputerReparatieShop.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ComputerReparatieShop.DAL.MyContext>
    {
        MyContext db = new MyContext();

        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ComputerReparatieShop.DAL.MyContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            db.Parts.Add(new PartModel
            {
                Name = "USB3-Poort",
                Price = 5.66,
                Manufacturer = "Henk",
                InStock = 10,
                Ordered = 0,
                NextDelivery = DateTime.Now,
            });

            db.Customers.Add(new CustomerModel
            {
                FirstName = "Test",
                LastName = "User",
                City = "KutStad",
                Country = "ShitHole",
                Adress = "Klotestraat 32",
                ZipCode = "1337 KS",
            });

            db.Employees.Add(new EmployeeModel
            {
                FirstName = "Bob",
                Preposition = "de",
                LastName = "Bouwer",
                City = "BobStad",
                Country = "BobLand",
                Adress = "Timmermanstraat 1",
                ZipCode = "2121 BB",
                Fee = 9.5,
            });

            //Save Bob de Bouwer and Test User so we can add them to the repairorders.
            db.SaveChanges();

            db.Repairs.Add(new RepairOrderModel
            {
                Customer = db.Customers.Where(x => x.FirstName == "Test").FirstOrDefault(),
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddDays(12),
                Description = "Eerste reparatie",
                HoursWorked = 0,
                Status = "In progress",
                Employee = db.Employees.Where(x => x.FirstName =="Bob").FirstOrDefault(),
                PartsUsed = db.Parts.ToList(),
            });

            db.Repairs.Add(new RepairOrderModel
            {
                Customer = db.Customers.Where(x => x.FirstName == "Test").FirstOrDefault(),
                StartDate = DateTime.Now.AddDays(-2),
                EndDate = DateTime.Now.AddDays(14),
                Description = "Tweede reparatie",
                HoursWorked = 0,
                Status = "Inactive",
                Employee = db.Employees.Where(x => x.FirstName =="Bob").FirstOrDefault(),
                PartsUsed = db.Parts.ToList(),
            });

            db.SaveChanges();
        }
    }
}
