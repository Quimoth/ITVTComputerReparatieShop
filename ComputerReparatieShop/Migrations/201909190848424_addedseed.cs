namespace ComputerReparatieShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedseed : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels");
            DropIndex("dbo.PartModels", new[] { "RepairOrderModel_RepairOrderId" });
            DropColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId", c => c.Int());
            CreateIndex("dbo.PartModels", "RepairOrderModel_RepairOrderId");
            AddForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels", "RepairOrderId");
        }
    }
}
