namespace Esso.Web.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _213213 : DbMigration
    {
		public override void Up()
		{
			AlterColumn("LOG724DB.TBL_STATION_STRING", "DISPLAY_NAME", c => c.String(maxLength:50, nullable: false));
		}

		public override void Down()
		{
			
		}
	}
}
