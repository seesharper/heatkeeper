using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Abstractions.CQRS
{
    /// <summary>
    /// Represents a class that is capable of executing a query
    /// </summary>
    public interface IQueryExecutor
    {
        /// <summary>
        /// Executes the given <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
        /// <param name="query">The query to be executed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result from the query.</returns>
        Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default(CancellationToken));
    }
}