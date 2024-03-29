namespace HeatKeeper.Server.Users
{
    public class CreateUserCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider) : ICommandHandler<CreateUserCommand>
    {
        public async Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.InsertUser, command);
            command.Id = await dbConnection.ExecuteScalarAsync<long>(sqlProvider.GetUserId, new { command.Email });
        }
    }

    [RequireAdminRole]
    public class CreateUserCommand
    {
        public long Id { get; set; }

        public string Email { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsAdmin { get; set; }

        public string HashedPassword { get; set; }
    }
}
