namespace MyScore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MergeCourseAndScoreEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Scores", "ID", "dbo.Courses");
            DropIndex("dbo.Scores", new[] { "ID" });
            AddColumn("dbo.Courses", "Score", c => c.Double());
            DropTable("dbo.Scores");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        ID = c.Int(nullable: false),
                        AverageScore = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropColumn("dbo.Courses", "Score");
            CreateIndex("dbo.Scores", "ID");
            AddForeignKey("dbo.Scores", "ID", "dbo.Courses", "ID");
        }
    }
}
