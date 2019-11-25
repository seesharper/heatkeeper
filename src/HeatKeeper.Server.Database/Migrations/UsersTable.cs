using FluentMigrator;

namespace HeatKeeper.Server.Database.Migrations
{
    [Migration(5)]
    public class AddUsersTable : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Id").AsInt64().PrimaryKey().Identity()
                .WithColumn("Name").AsString(255).Unique("idx_users_name").NotNullable()
                .WithColumn("Email").AsString(255)
                .WithColumn("IsAdmin").AsBoolean().NotNullable()
                .WithColumn("HashedPassword").AsString(255).NotNullable();
            Insert.IntoTable("Users").Row(new { Name = AdminUser.UserName, Email = AdminUser.DefaultEmail, IsAdmin = true, HashedPassword = AdminUser.DefaultPasswordHash });
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
