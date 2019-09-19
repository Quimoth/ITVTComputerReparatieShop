namespace ComputerReparatieShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId", c => c.Int());
            CreateIndex("dbo.PartModels", "RepairOrderModel_RepairOrderId");
            AddForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels", "RepairOrderId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels");
            DropIndex("dbo.PartModels", new[] { "RepairOrderModel_RepairOrderId" });
            DropColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId");
        }
    }
}
