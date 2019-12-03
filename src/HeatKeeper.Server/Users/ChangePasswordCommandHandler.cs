using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Authorization;
using System.Threading;
using System.Threading.Tasks;

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
    public class ChangePasswordCommand
    {
        public ChangePasswordCommand(string oldPassword, string newPassword, string confirmedPassword)
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
            ConfirmedPassword = confirmedPassword;
        }

        public string NewPassword { get; }
        public string ConfirmedPassword { get; }
        public string OldPassword { get; }
    }
}
