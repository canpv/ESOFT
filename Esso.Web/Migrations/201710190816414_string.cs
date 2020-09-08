namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _string : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "LOG724DB.TBL_STATION_STRING",
            //    c => new
            //        {
            //            ID = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
            //            STATION_ID = c.Decimal(nullable: false, precision: 10, scale: 0),
            //            STRING_ID = c.Decimal(nullable: false, precision: 10, scale: 0),
            //            IS_DELETED = c.Decimal(nullable: false, precision: 1, scale: 0),
            //            UPDATE_USER = c.String(maxLength: 128),
            //            CREATED_DATE = c.DateTime(),
            //            UPDATED_DATE = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            //DropTable("LOG724DB.TBL_STATION_STRING");
        }
    }
}
