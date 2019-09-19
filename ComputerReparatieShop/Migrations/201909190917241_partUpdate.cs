namespace ComputerReparatieShop.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class partUpdate : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels");
            DropIndex("dbo.PartModels", new[] { "RepairOrderModel_RepairOrderId" });
            CreateTable(
                "dbo.PartModelRepairOrderModels",
                c => new
                    {
                        PartModel_PartId = c.Int(nullable: false),
                        RepairOrderModel_RepairOrderId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PartModel_PartId, t.RepairOrderModel_RepairOrderId })
                .ForeignKey("dbo.PartModels", t => t.PartModel_PartId, cascadeDelete: true)
                .ForeignKey("dbo.RepairOrderModels", t => t.RepairOrderModel_RepairOrderId, cascadeDelete: true)
                .Index(t => t.PartModel_PartId)
                .Index(t => t.RepairOrderModel_RepairOrderId);
            
            DropColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PartModels", "RepairOrderModel_RepairOrderId", c => c.Int());
            DropForeignKey("dbo.PartModelRepairOrderModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels");
            DropForeignKey("dbo.PartModelRepairOrderModels", "PartModel_PartId", "dbo.PartModels");
            DropIndex("dbo.PartModelRepairOrderModels", new[] { "RepairOrderModel_RepairOrderId" });
            DropIndex("dbo.PartModelRepairOrderModels", new[] { "PartModel_PartId" });
            DropTable("dbo.PartModelRepairOrderModels");
            CreateIndex("dbo.PartModels", "RepairOrderModel_RepairOrderId");
            AddForeignKey("dbo.PartModels", "RepairOrderModel_RepairOrderId", "dbo.RepairOrderModels", "RepairOrderId");
        }
    }
}
