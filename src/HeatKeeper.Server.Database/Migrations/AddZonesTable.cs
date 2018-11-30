using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(1)]
    public class AddZonesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Zones")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Description").AsString();
        }

        public override void Down()
        {
            Delete.Table("Zones");
        }
    }
}
