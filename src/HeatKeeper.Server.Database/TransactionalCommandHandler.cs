using System.Data;
using System.Threading;
using System.Threading.Tasks;
using CQRS.Command.Abstractions;
using Microsoft.Extensions.Logging;

namespace HeatKeeper.Abstractions.Transactions
{
    public class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly IDbConnection _dbConnection;
        private readonly ICommandHandler<TCommand> _commandHandler;

        private readonly ILogger<TransactionalCommandHandler<TCommand>> _logger;

        public TransactionalCommandHandler(IDbConnection dbConnection, ICommandHandler<TCommand> commandHandler, ILogger<TransactionalCommandHandler<TCommand>> logger)
        {
            _dbConnection = dbConnection;
            _commandHandler = commandHandler;
            _logger = logger;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Starting transaction for {typeof(TCommand)}", typeof(TCommand));
            using (var transaction = _dbConnection.BeginTransaction())
            {
                await _commandHandler.HandleAsync(command, cancellationToken);
                transaction.Commit();
                _logger.LogDebug("Transaction committed for {typeof(TCommand)}", typeof(TCommand));
            }
        }
    }
}
