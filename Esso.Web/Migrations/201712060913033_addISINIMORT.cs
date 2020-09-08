namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addISINIMORT : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.TBL_OZET", "ISINIM_ORT", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.TBL_OZET", "ISINIM_ORT");
        }
    }
}
