using CQRS.Command.Abstractions;
using HeatKeeper.Server.Database;
using HeatKeeper.Server.Security;
using HeatKeeper.Server.Users;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

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

            var adduserCommand = new InsertUserLocationCommand(userContext.Id, insertLocationCommand.Id);
            await commandExecutor.ExecuteAsync(adduserCommand).ConfigureAwait(false);

            command.Id = insertLocationCommand.Id;
        }
    }

    public class LocationCommand
    {
        public LocationCommand(string name, string description)
        {
            Name = name;
            Description = description;
        }

        public string Name { get; }

        public string Description { get; }

        public long Id { get; set; }
    }


    [RequireAdminRole]
    public class CreateLocationCommand : LocationCommand
    {
        public CreateLocationCommand(string name, string description) : base(name, description)
        {
        }
    }

}
