using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Abstractions.Transactions;

public class TransactionalCommandHandler<TCommand>(IDbConnection dbConnection, ICommandHandler<TCommand> commandHandler, ILogger<TransactionalCommandHandler<TCommand>> logger) : ICommandHandler<TCommand>
{
    public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
    {
        logger.LogDebug("Starting transaction for {typeof(TCommand)}", typeof(TCommand));
        using (var transaction = dbConnection.BeginTransaction())
        {
            await commandHandler.HandleAsync(command, cancellationToken);
            transaction.Commit();
            logger.LogDebug("Transaction committed for {typeof(TCommand)}", typeof(TCommand));
        }
    }
}
