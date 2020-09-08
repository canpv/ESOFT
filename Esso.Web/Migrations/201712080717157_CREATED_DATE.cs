namespace Esso.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CREATED_DATE : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.AspNetUsers", "CREATED_DATE", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.AspNetUsers", "CREATED_DATE");
        }
    }
}
