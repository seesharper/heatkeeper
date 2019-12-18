using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
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
            command.UserId = createUserCommand.Id;
        }
    }

    [RequireAdminRole]
    public class RegisterUserCommand : UserCommand
    {
        public string Password { get; set; }

        public string ConfirmedPassword { get; set; }
    }
}
