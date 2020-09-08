namespace Esso.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class es120220182 : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.AspNetUsers", "SHOW_MONEY", c => c.Decimal(nullable: true, precision: 10, scale: 0));
        }

        public override void Down()
        {
        }
    }
}
