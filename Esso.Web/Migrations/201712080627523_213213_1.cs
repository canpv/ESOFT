namespace Esso.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _213213_1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.AspNetUsers", "IS_DEMO", c => c.Decimal(precision: 1, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.AspNetUsers", "IS_DEMO");
        }
    }
}
