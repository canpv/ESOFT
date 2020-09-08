namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class aaaae : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.TBL_STATION", "METEROROLOGY_PLANT", c => c.Decimal(precision: 10, scale: 0));
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.TBL_STATION", "METEROROLOGY_PLANT");
        }
    }
}
