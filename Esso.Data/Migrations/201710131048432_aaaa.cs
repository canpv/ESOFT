namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aaaa : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.TBL_STATION", "IS_CENTRAL_INV", c => c.Decimal(precision: 1, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.TBL_STATION", "IS_CENTRAL_INV");
        }
    }
}
