using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Security;

namespace HeatKeeper.Server.Users
{
    public class ChangePasswordValidator : ICommandHandler<ChangePasswordCommand>
    {
        private readonly ICommandHandler<ChangePasswordCommand> handler;
        private readonly IQueryExecutor queryExecutor;
        private readonly IUserContext userContext;
        private readonly IPasswordPolicy passwordPolicy;

        public ChangePasswordValidator(ICommandHandler<ChangePasswordCommand> handler, IQueryExecutor queryExecutor, IUserContext userContext, IPasswordPolicy passwordPolicy)
        {
            this.handler = handler;
            this.queryExecutor = queryExecutor;
            this.userContext = userContext;
            this.passwordPolicy = passwordPolicy;
        }

        public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await queryExecutor.ExecuteAsync(new AuthenticatedUserQuery(userContext.Name, command.OldPassword));
            if (string.Equals(command.NewPassword, command.OldPassword))
            {
                throw new HeatKeeperSecurityException("The new password must be different from the old password");
            }

            passwordPolicy.Apply(command.NewPassword, command.ConfirmedPassword);

            await handler.HandleAsync(command).ConfigureAwait(false);
        }
    }
}