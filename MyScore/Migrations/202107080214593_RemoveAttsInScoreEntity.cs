namespace MyScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAttsInScoreEntity : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Scores", "MidtermScore");
            DropColumn("dbo.Scores", "EndingScore");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Scores", "EndingScore", c => c.Double(nullable: false));
            AddColumn("dbo.Scores", "MidtermScore", c => c.Double(nullable: false));
        }
    }
}
