namespace MyScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingOrderNumberToCourseEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Courses", "OrderNumber", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Courses", "OrderNumber");
        }
    }
}
