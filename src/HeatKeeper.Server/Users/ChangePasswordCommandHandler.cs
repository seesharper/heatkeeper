using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Users
{
    public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand>
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IPasswordManager passwordManager;
        private readonly IUserContext userContext;

        public ChangePasswordCommandHandler(ICommandExecutor commandExecutor, IPasswordManager passwordManager, IUserContext userContext)
        {
            this.commandExecutor = commandExecutor;
            this.passwordManager = passwordManager;
            this.userContext = userContext;
        }

        public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
        {
            var hashedPassword = passwordManager.GetHashedPassword(command.NewPassword);
            await commandExecutor.ExecuteAsync(new UpdatePasswordHashCommand(userContext.Id, hashedPassword));
        }
    }

    [RequireUserRole]
    public record ChangePasswordCommand(string OldPassword, string NewPassword, string ConfirmedPassword);
}
