using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(2)]
    public class AddZonesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Zones")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("LocationId").AsInt64().ReferencedBy("Locations", "Id")
                .WithColumn("Name").AsString(255).Unique("idx_zones_name")
                .WithColumn("Description").AsString();
        }

        public override void Down()
        {
            Delete.Table("Zones");
        }
    }
}
