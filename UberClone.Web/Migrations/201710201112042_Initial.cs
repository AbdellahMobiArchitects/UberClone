namespace UberClone.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Requests",
                c => new
                    {
                        Requestid = c.Int(nullable: false, identity: true),
                        Requester_username = c.String(),
                        Driver_username = c.String(),
                        Requester_lat = c.Double(nullable: false),
                        Requester_long = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.Requestid);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        User_id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Usertype = c.String(),
                        User_long = c.Double(nullable: false),
                        User_Lat = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.User_id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
            DropTable("dbo.Requests");
        }
    }
}
