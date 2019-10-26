using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Abstractions.CQRS
{
    /// <summary>
    /// Represents a class that is capable of executing a command.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes the given <paramref name="command"/>.
        /// </summary>
        /// <typeparam name="TCommand">The type of command to be executed.</typeparam>
        /// <param name="command">The command to be executed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default);
    }

    public interface IBus
    {
       Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken));

       Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class Bus : IBus
    {
        private readonly ICommandExecutor commandExecutor;
        private readonly IQueryExecutor queryExecutor;

        public Bus(ICommandExecutor commandExecutor, IQueryExecutor queryExecutor)
        {
            this.commandExecutor = commandExecutor;
            this.queryExecutor = queryExecutor;
        }

        public async Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken))
        {
            await commandExecutor.ExecuteAsync(command, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return await queryExecutor.ExecuteAsync(query, cancellationToken).ConfigureAwait(false);
        }
    }
}