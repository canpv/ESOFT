namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class uytnm : DbMigration
    {
        public override void Up()
        {
            //CreateTable(
            //    "LOG724DB.TBL_PUSH_NTF",
            //    c => new
            //    {
            //        ID = c.Decimal(nullable: false, precision: 10, scale: 0, identity: true),
            //        USER_ID = c.String(nullable: false, maxLength: 128),
            //        USER_TOKEN = c.Decimal(nullable: false, precision: 10, scale: 0),
            //        PUSH_TOKEN = c.Decimal(nullable: false, precision: 10, scale: 0),
            //        IS_IOS = c.Decimal(nullable: false, precision: 1, scale: 0),
            //        EXPRIRED = c.Decimal(nullable: false, precision: 1, scale: 0),
            //        UPDATE_USER = c.String(maxLength: 128),
            //        INSERT_DATE = c.DateTime(),
            //    })
            //    .PrimaryKey(t => t.ID);
        }

        public override void Down()
        {
            //DropTable("LOG724DB.TBL_PUSH_NTF");
        }
    }
}
