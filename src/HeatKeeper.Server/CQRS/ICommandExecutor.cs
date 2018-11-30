using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.CQRS
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
        Task ExecuteAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}