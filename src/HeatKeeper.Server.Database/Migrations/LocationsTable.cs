using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(2)]
    public class AddLocationsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Locations")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("SensorId").AsInt64().ReferencedBy("Sensors", "Id").Nullable()
                .WithColumn("Name").AsString(255).Unique("idx_locations_name").NotNullable()
                .WithColumn("Description").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Locations");
        }
    }
}