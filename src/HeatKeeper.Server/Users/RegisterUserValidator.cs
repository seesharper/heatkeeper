using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;

namespace HeatKeeper.Server.Users
{
    public class RegisterUserValidator : ICommandHandler<RegisterUserCommand>
    {
        private readonly ICommandHandler<RegisterUserCommand> handler;
        private readonly IPasswordPolicy passwordPolicy;

        public RegisterUserValidator(ICommandHandler<RegisterUserCommand> handler, IPasswordPolicy passwordPolicy)
        {
            this.handler = handler;
            this.passwordPolicy = passwordPolicy;
        }

        public async Task HandleAsync(RegisterUserCommand command, CancellationToken cancellationToken = default)
        {
            passwordPolicy.Apply(command.Password, command.ConfirmedPassword);
            await handler.HandleAsync(command, cancellationToken);
        }
    }
}
