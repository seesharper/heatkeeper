using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Authentication;
using HeatKeeper.Server.Exceptions;

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

        public async Task HandleAsync(ChangePasswordCommand command, CancellationToken cancellationToken = default)
        {
            await queryExecutor.ExecuteAsync(new AuthenticatedUserQuery(userContext.Email, command.OldPassword));
            if (string.Equals(command.NewPassword, command.OldPassword))
            {
                throw new AuthenticationFailedException("The new password must be different from the old password");
            }

            passwordPolicy.Apply(command.NewPassword, command.ConfirmedPassword);

            await handler.HandleAsync(command).ConfigureAwait(false);
        }
    }
}
