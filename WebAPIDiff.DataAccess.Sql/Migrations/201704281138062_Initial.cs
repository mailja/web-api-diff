namespace WebAPIDiff.DataAccess.Sql.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Diff",
                c => new
                    {
                        DiffId = c.Int(nullable: false),
                        LeftData = c.String(maxLength: 500),
                        RightData = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.DiffId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Diff");
        }
    }
}
