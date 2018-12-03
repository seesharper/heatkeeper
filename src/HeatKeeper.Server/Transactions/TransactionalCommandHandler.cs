using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Server.CQRS;

namespace HeatKeeper.Server.Database.Transactions
{
    public class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
{
    private readonly IDbConnection dbConnection;
    private readonly ICommandHandler<TCommand> commandHandler;

    public TransactionalCommandHandler(IDbConnection dbConnection, ICommandHandler<TCommand> commandHandler)
    {
        this.dbConnection = dbConnection;
        this.commandHandler = commandHandler;
    }

    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        using (var transaction = dbConnection.BeginTransaction())
        {
            await commandHandler.HandleAsync(command, cancellationToken);
            transaction.Commit();
        }                
    }
}
}