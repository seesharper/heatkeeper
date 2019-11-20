using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HeatKeeper.Abstractions.CQRS;
using HeatKeeper.Abstractions.Logging;

namespace HeatKeeper.Abstractions.Transactions
{
    public class TransactionalCommandHandler<TCommand> : ICommandHandler<TCommand>
    {
        private readonly IDbConnection dbConnection;
        private readonly ICommandHandler<TCommand> commandHandler;
        private readonly Logger logger;

        public TransactionalCommandHandler(IDbConnection dbConnection, ICommandHandler<TCommand> commandHandler, Logger logger)
        {
            this.dbConnection = dbConnection;
            this.commandHandler = commandHandler;
            this.logger = logger;
        }

        public async Task HandleAsync(TCommand command, CancellationToken cancellationToken)
        {
            logger.Debug($"Starting transaction for {typeof(TCommand)}");
            using (var transaction = dbConnection.BeginTransaction())
            {
                await commandHandler.HandleAsync(command, cancellationToken);
                transaction.Commit();
                logger.Debug($"Transaction committed for {typeof(TCommand)}");
            }
        }
    }
}
