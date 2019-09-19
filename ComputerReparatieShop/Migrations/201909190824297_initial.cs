namespace ComputerReparatieShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CustomerModels",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        Preposition = c.String(),
                        LastName = c.String(),
                        Adress = c.String(),
                        ZipCode = c.String(),
                        Country = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.CustomerId);
            
            CreateTable(
                "dbo.RepairOrderModels",
                c => new
                    {
                        RepairOrderId = c.Int(nullable: false, identity: true),
                        Status = c.String(),
                        StartDate = c.DateTime(),
                        Description = c.String(),
                        EndDate = c.DateTime(),
                        HoursWorked = c.Double(nullable: false),
                        Customer_CustomerId = c.Int(),
                        Employee_EmployeeId = c.Int(),
                    })
                .PrimaryKey(t => t.RepairOrderId)
                .ForeignKey("dbo.CustomerModels", t => t.Customer_CustomerId)
                .ForeignKey("dbo.EmployeeModels", t => t.Employee_EmployeeId)
                .Index(t => t.Customer_CustomerId)
                .Index(t => t.Employee_EmployeeId);
            
            CreateTable(
                "dbo.EmployeeModels",
                c => new
                    {
                        EmployeeId = c.Int(nullable: false, identity: true),
                        Fee = c.Double(nullable: false),
                        FirstName = c.String(),
                        Preposition = c.String(),
                        LastName = c.String(),
                        Adress = c.String(),
                        ZipCode = c.String(),
                        Country = c.String(),
                        City = c.String(),
                    })
                .PrimaryKey(t => t.EmployeeId);
            
            CreateTable(
                "dbo.PartModels",
                c => new
                    {
                        PartId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Double(nullable: false),
                        InStock = c.Int(nullable: false),
                        Ordered = c.Int(nullable: false),
                        NextDelivery = c.DateTime(),
                        Manufacturer = c.String(),
                        RepairOrderModel_RepairOrderId = c.Int(),
                    })
                .PrimaryKey(t => t.PartId)
                .ForeignKey("dbo.RepairOrderModels", t => t.RepairOrderModel_RepairOrderId)
                .Index(t => t.RepairOrderModel_RepairOrderId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels");
            DropForeignKey("dbo.RepairOrderModels", "Employee_EmployeeId", "dbo.EmployeeModels");
            DropForeignKey("dbo.RepairOrderModels", "Customer_CustomerId", "dbo.CustomerModels");
            DropIndex("dbo.PartModels", new[] { "RepairOrderModel_RepairOrderId" });
            DropIndex("dbo.RepairOrderModels", new[] { "Employee_EmployeeId" });
            DropIndex("dbo.RepairOrderModels", new[] { "Customer_CustomerId" });
            DropTable("dbo.PartModels");
            DropTable("dbo.EmployeeModels");
            DropTable("dbo.RepairOrderModels");
            DropTable("dbo.CustomerModels");
        }
    }
}
