namespace Esso.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _5555 : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("LOG724DB.AspNetUsers", "IS_DEMO", c => c.Decimal(nullable: false, precision: 1, scale: 0));
        }

        public override void Down()
        {
            //AlterColumn("LOG724DB.AspNetUsers", "IS_DEMO", c => c.Decimal(precision: 1, scale: 0));
        }
    }
}
