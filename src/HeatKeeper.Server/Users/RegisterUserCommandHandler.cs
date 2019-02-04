using HeatKeeper.Abstractions.CQRS;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand>
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IPasswordManager passwordManager;

        public RegisterUserCommandHandler(ICommandExecutor commandExecutor, IPasswordManager passwordManager)
        {
            this.commandExecutor = commandExecutor;
            this.passwordManager = passwordManager;
        }

        public async Task HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            var hashedPassword = passwordManager.GetHashedPassword(command.Password);
            var createUserCommand = new CreateUserCommand(command.Name, command.Email, hashedPassword, command.IsAdmin);
            await commandExecutor.ExecuteAsync(createUserCommand);
            command.Id = createUserCommand.Id;
        }
    }

    public class RegisterUserCommand
    {
        public RegisterUserCommand(string name, string email, string password, bool isAdmin)
        {
            Name = name;
            Email = email;
            Password = password;
            IsAdmin = isAdmin;
        }

        public string Name { get; }
        public string Email { get; }
        public string Password { get; }
        public bool IsAdmin { get; }
        public long Id { get; set;}
    }
}