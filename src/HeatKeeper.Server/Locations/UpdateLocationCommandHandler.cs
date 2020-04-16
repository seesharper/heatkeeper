using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using DbReader;
using HeatKeeper.Server.Authorization;
using HeatKeeper.Server.Database;

namespace HeatKeeper.Server.Locations
{
    public class UpdateLocationCommandHandler : ICommandHandler<UpdateLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public UpdateLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(UpdateLocationCommand command, CancellationToken cancellationToken = default)
        {
            await dbConnection.ExecuteAsync(sqlProvider.UpdateLocation, command).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Updates the name and the description for the given location.
    /// </summary>
    [RequireAdminRole]
    public class UpdateLocationCommand : LocationCommand
    {
    }
}
