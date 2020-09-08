namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uyt : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "LOG724DB.TBL_PUSH_NTF",
            //    c => new
            //        {
            //            ID = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
            //            USER_ID = c.String(nullable: false, maxLength: 128),
            //            USER_TOKEN = c.Decimal(nullable: false, precision: 10, scale: 0),
            //            PUSH_TOKEN = c.Decimal(nullable: false, precision: 10, scale: 0),
            //            IS_IOS = c.Decimal(nullable: false, precision: 1, scale: 0),
            //            EXPRIRED = c.Decimal(nullable: false, precision: 1, scale: 0),
            //            UPDATE_USER = c.String(maxLength: 128),
            //            INSERT_DATE = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //CreateTable(
            //    "LOG724DB.TBL_TARGET",
            //    c => new
            //        {
            //            ID = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
            //            STATION_ID = c.Decimal(nullable: false, precision: 10, scale: 0),
            //            JAN_PRODUCTION = c.Single(),
            //            FEB_PRODUCTION = c.Single(),
            //            MARCH_PRODUCTION = c.Single(),
            //            APRIL_PRODUCTION = c.Single(),
            //            MAY_PRODUCTION = c.Single(),
            //            JUNE_PRODUCTION = c.Single(),
            //            JULY_PRODUCTION = c.Single(),
            //            AUGUST_PRODUCTION = c.Single(),
            //            SEP_PRODUCTION = c.Single(),
            //            OKT_PRODUCTION = c.Single(),
            //            NOV_PRODUCTION = c.Single(),
            //            DEC_PRODUCTION = c.Single(),
            //            YEAR_PRODUCTION = c.Single(),
            //            JAN_IRRADIATION = c.Single(),
            //            FEB_IRRADIATION = c.Single(),
            //            MARCH_IRRADIATION = c.Single(),
            //            APRIL_IRRADIATION = c.Single(),
            //            MAY_IRRADIATION = c.Single(),
            //            JUNE_IRRADIATION = c.Single(),
            //            JULY_IRRADIATION = c.Single(),
            //            AUGUST_IRRADIATION = c.Single(),
            //            SEP_IRRADIATION = c.Single(),
            //            OKT_IRRADIATION = c.Single(),
            //            NOV_IRRADIATION = c.Single(),
            //            DEC_IRRADIATION = c.Single(),
            //            YEAR_IRRADIATION = c.Single(),
            //            IS_DELETED = c.Decimal(nullable: false, precision: 1, scale: 0),
            //            UPDATE_USER = c.String(maxLength: 128),
            //            INSTALL_DATE = c.DateTime(),
            //            CREATED_DATE = c.DateTime(),
            //            UPDATED_DATE = c.DateTime(),
            //        })
            //    .PrimaryKey(t => t.ID);
            
            //AddColumn("LOG724DB.TBL_ALARM_LOG", "PUSH_STAT", c => c.Decimal(precision: 1, scale: 0));
        }
        
        public override void Down()
        {
            //DropColumn("LOG724DB.TBL_ALARM_LOG", "PUSH_STAT");
            //DropTable("LOG724DB.TBL_TARGET");
            //DropTable("LOG724DB.TBL_PUSH_NTF");
        }
    }
}
