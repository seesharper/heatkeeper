using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(1)]
    public class AddSensorsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Sensors")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("ExternalId").AsString().Unique().NotNullable()
                .WithColumn("Name").AsString(255).Unique("idx_sensors_name")
                .WithColumn("Description").AsString().Nullable();
        }

        public override void Down()
        {
            Delete.Table("Sensors");
        }
    }
}