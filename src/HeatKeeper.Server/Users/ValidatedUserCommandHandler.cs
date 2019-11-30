using CQRS.Command.Abstractions;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Exceptions;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Users
{
    public class ValidatedUserCommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : UserCommand
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IEmailValidator emailValidator;
        private readonly IQueryExecutor queryExecutor;

        public ValidatedUserCommandHandler(ICommandHandler<TCommand> handler, IEmailValidator emailValidator, IQueryExecutor queryExecutor)
        {
            this.handler = handler;
            this.emailValidator = emailValidator;
            this.queryExecutor = queryExecutor;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            emailValidator.Validate(command.Email);
            var userExists = await queryExecutor.ExecuteAsync(new UserExistsQuery(command.UserId, command.Email));
            if (userExists)
            {
                throw new HeatKeeperConflictException($"User {command.Email} already exists");
            }
            await handler.HandleAsync(command);
        }
    }

}
