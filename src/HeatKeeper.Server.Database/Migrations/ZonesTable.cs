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
                .WithColumn("Name").AsString(255).NotNullable()
                .WithColumn("Description").AsString().Nullable();
            Create.Index("idx_zones").OnTable("Zones").OnColumn("LocationId").Ascending().OnColumn("Name").Ascending().WithOptions().Unique();

        }

        public override void Down()
        {
            Delete.Table("Zones");
        }
    }
}
