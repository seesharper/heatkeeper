using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(3)]
    public class AddZonesTable : Migration
    {
        public override void Up()
        {
            Create.Table("Zones")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("LocationId").AsInt64().ReferencedBy("Locations", "Id").NotNullable()
                .WithColumn("SensorId").AsInt64().ReferencedBy("Sensors", "Id").Nullable()
                .WithColumn("Name").AsString(255).Unique("idx_zones_name").NotNullable()
                .WithColumn("Description").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Zones");
        }
    }
}
