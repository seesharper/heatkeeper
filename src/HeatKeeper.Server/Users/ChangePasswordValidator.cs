using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class ChangePasswordValidator : ICommandHandler<ChangePasswordCommand>
    {
        private readonly ICommandHandler<ChangePasswordCommand> handler;
        private readonly ICommandExecutor commandExecutor;
        private readonly IUserContext userContext;
        private readonly IPasswordPolicy passwordPolicy;

        public ChangePasswordValidator(ICommandHandler<ChangePasswordCommand> handler, ICommandExecutor commandExecutor, IUserContext userContext, IPasswordPolicy passwordPolicy)
        {
            this.handler = handler;
            this.commandExecutor = commandExecutor;
            this.userContext = userContext;
            this.passwordPolicy = passwordPolicy;
        }

        public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await commandExecutor.ExecuteAsync(new AuthenticateCommand(userContext.Name, command.OldPassword));
            if (string.Equals(command.NewPassword, command.OldPassword))
            {
                throw new HeatKeeperSecurityException("The new password must be different from the old password");
            }

            passwordPolicy.Apply(command.NewPassword);

            await handler.HandleAsync(command).ConfigureAwait(false);
        }
    }
}