using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Security;

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
            var createUserCommand = new CreateUserCommand() { Email = command.Email, FirstName = command.FirstName, LastName = command.LastName, IsAdmin = command.IsAdmin, HashedPassword = hashedPassword };
            await commandExecutor.ExecuteAsync(createUserCommand);
            command.Id = createUserCommand.UserId;
        }
    }

    [RequireAdminRole]
    public class RegisterUserCommand
    {
        public RegisterUserCommand(string email, string firstName, string lastName, bool isAdmin, string password, string confirmedPassword)
        {
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            IsAdmin = isAdmin;
            Password = password;
            ConfirmedPassword = confirmedPassword;
        }

        public string Name { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public bool IsAdmin { get; }
        public string Password { get; }
        public string ConfirmedPassword { get; }
        public long Id { get; set; }
    }
}
