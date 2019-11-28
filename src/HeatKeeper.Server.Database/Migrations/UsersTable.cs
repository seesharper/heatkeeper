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
                .WithColumn("Email").AsString(255).Unique("idx_users_email").NotNullable()
                .WithColumn("FirstName").AsString(255).NotNullable()
                .WithColumn("LastName").AsString(255).NotNullable()
                .WithColumn("IsAdmin").AsBoolean().NotNullable()
                .WithColumn("HashedPassword").AsString(255).NotNullable();
            Insert.IntoTable("Users").Row(new { Email = AdminUser.DefaultEmail, FirstName = AdminUser.DefaultFirstName, LastName = AdminUser.DefaultLastName, IsAdmin = true, HashedPassword = AdminUser.DefaultPasswordHash });
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}
