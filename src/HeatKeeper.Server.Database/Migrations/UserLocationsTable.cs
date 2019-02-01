using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(6)]
    public class UserLocationsTable : Migration
    {
        public override void Up()
        {
            Create.Table("UserLocations")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("LocationId").AsInt64().NotNullable().ReferencedBy("Location", "Id")
                .WithColumn("UserId").AsInt64().NotNullable().ReferencedBy("Users", "Id");

            Create.Index("idx_user_locations")
                .OnTable("UserLocations")
                    .OnColumn("LocationId").Ascending()
                    .OnColumn("UserId").Ascending()
                    .WithOptions().Unique();
        }

         public override void Down()
        {
            Delete.Table("UserLocations");
        }
    }
}