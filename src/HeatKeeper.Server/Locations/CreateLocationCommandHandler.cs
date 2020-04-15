using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Locations
{
    public class CreateLocationCommandHandler : ICommandHandler<CreateLocationCommand>
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IUserContext userContext;

        public CreateLocationCommandHandler(ICommandExecutor commandHandler, IUserContext userContext)
        {
            this.commandExecutor = commandHandler;
            this.userContext = userContext;
        }

        public async Task HandleAsync(CreateLocationCommand command, CancellationToken cancellationToken = default)
        {
            var insertLocationCommand = new InsertLocationCommand(command.Name, command.Description);
            await commandExecutor.ExecuteAsync(insertLocationCommand).ConfigureAwait(false);

            var adduserCommand = new AddUserToLocationCommand(userContext.Id) { LocationId = insertLocationCommand.Id };
            await commandExecutor.ExecuteAsync(adduserCommand).ConfigureAwait(false);

            command.Id = insertLocationCommand.Id;
        }
    }

    public class LocationCommand
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public long Id { get; set; }
    }


    /// <summary>
    /// Creates a new location.
    /// </summary>
    [RequireAdminRole]
    public class CreateLocationCommand : LocationCommand
    {
    }

}
