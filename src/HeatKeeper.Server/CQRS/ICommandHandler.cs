using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Server.CQRS
{
    /// <summary>
    /// Represents a class that is capable of handling a <typeparamref name="TCommand"/>.
    /// </summary>
    /// <typeparam name="TCommand">The type of command to handle.</typeparam>
    public interface ICommandHandler<in TCommand>
    {
        /// <summary>
        /// Handles the given <paramref name="command"/>.
        /// </summary>
        /// <param name="command">THe command to handle.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns><see cref="Task"/>.</returns>
        Task HandleAsync(TCommand command, CancellationToken cancellationToken = default(CancellationToken));
    }
}