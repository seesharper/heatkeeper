using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(4)]
    public class AddMeasurementsTable : Migration
    {
        public override void Up()
        {
            Create.Table("Measurements")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("SensorId").AsInt64().ReferencedBy("Sensors", "Id").NotNullable()
                .WithColumn("MeasurementType").AsByte().NotNullable()
                .WithColumn("Value").AsDouble().NotNullable()
                .WithColumn("Exported").AsDateTime().Nullable()
                .WithColumn("Created").AsDateTime().NotNullable().WithDefault(SystemMethods.CurrentUTCDateTime);
        }

        public override void Down()
        {
            Delete.Table("Measurements");
        }
    }
}