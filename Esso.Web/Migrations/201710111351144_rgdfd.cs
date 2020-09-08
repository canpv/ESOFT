namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rgdfd : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.TBL_PUSH_NTF", "USER_TOKEN", c => c.String(nullable: false));
            AddColumn("LOG724DB.TBL_PUSH_NTF", "PUSH_TOKEN", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.TBL_PUSH_NTF", "PUSH_TOKEN");
            DropColumn("LOG724DB.TBL_PUSH_NTF", "USER_TOKEN");
        }
    }
}
