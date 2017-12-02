namespace OpenNos.DAL.EF.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class Aphrodite78 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.LogVip",
                c => new
                    {
                        LogId = c.Long(nullable: false, identity: true),
                        AccountId = c.Long(),
                        CharacterId = c.Long(),
                        Timestamp = c.DateTime(nullable: false),
                        VipPack = c.String(),
                    })
                .PrimaryKey(t => t.LogId)
                .ForeignKey("dbo.Account", t => t.AccountId)
                .ForeignKey("dbo.Character", t => t.CharacterId)
                .Index(t => t.AccountId)
                .Index(t => t.CharacterId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.LogVip", "CharacterId", "dbo.Character");
            DropForeignKey("dbo.LogVip", "AccountId", "dbo.Account");
            DropIndex("dbo.LogVip", new[] { "CharacterId" });
            DropIndex("dbo.LogVip", new[] { "AccountId" });
            DropTable("dbo.LogVip");
        }
    }
}
