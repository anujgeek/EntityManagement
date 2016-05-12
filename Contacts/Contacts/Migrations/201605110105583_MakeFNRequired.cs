namespace Contacts.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeFNRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Contacts", "first_name", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Contacts", "first_name", c => c.String(maxLength: 50));
        }
    }
}
