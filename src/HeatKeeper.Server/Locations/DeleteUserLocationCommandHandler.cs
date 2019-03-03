using DbReader;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Server.Database;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.Locations
{
    public class DeleteUserLocationCommandHandler : ICommandHandler<DeleteUserLocationCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ISqlProvider sqlProvider;

        public DeleteUserLocationCommandHandler(IDbConnection dbConnection, ISqlProvider sqlProvider)
        {
            this.dbConnection = dbConnection;
            this.sqlProvider = sqlProvider;
        }

        public async Task HandleAsync(DeleteUserLocationCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await dbConnection.ExecuteAsync(sqlProvider.DeleteUserLocation, command);
        }
    }

    public class DeleteUserLocationCommand
    {
        public DeleteUserLocationCommand(long id)
        {
            Id = id;
        }
        public long Id { get; }
    }
}