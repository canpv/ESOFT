namespace Esso.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPR : DbMigration
    {
        public override void Up()
        {
            AddColumn("LOG724DB.TBL_OZET", "PR", c => c.Single());
        }
        
        public override void Down()
        {
            DropColumn("LOG724DB.TBL_OZET", "PR");
        }
    }
}
