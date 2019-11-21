using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

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

        public async Task HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            var hashedPassword = passwordManager.GetHashedPassword(command.Password);
            var createUserCommand = new CreateUserCommand(command.Name, command.Email, command.IsAdmin, hashedPassword);
            await commandExecutor.ExecuteAsync(createUserCommand);
            command.Id = createUserCommand.Id;
        }
    }

    public class RegisterUserCommand
    {
        public RegisterUserCommand(string name, string email, bool isAdmin, string password, string confirmedPassword)
        {
            Name = name;
            Email = email;
            IsAdmin = isAdmin;
            Password = password;
            ConfirmedPassword = confirmedPassword;
        }

        public string Name { get; }
        public string Email { get; }
        public bool IsAdmin { get; }
        public string Password { get; }
        public string ConfirmedPassword { get; }
        public long Id { get; set; }
    }
}
