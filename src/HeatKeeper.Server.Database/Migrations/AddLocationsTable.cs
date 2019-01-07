using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(1)]
    public class AddLocationsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Locations")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).Unique("idx_locations_name")
                .WithColumn("Description").AsString();
        }

        public override void Down()
        {
            Delete.Table("Locations");
        }
    }
}