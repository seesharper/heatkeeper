using System.Threading;
using System.Threading.Tasks;

namespace HeatKeeper.Abstractions.CQRS
{
    /// <summary>
    /// Represents a class that is capable of handling a query of
    /// type <typeparamref name="TQuery"/> and return a result of type <typeparamref name="TResult"/>.
    /// </summary>
    /// <typeparam name="TQuery">The type of query to handle.</typeparam>
    /// <typeparam name="TResult">The type of result to be returned.</typeparam>
    public interface IQueryHandler<in TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
        /// <summary>
        /// Handles the given <paramref name="query"/> and returns a result of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <param name="query">The query to be handled.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result of the query.</returns>
        Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken = default);
    }
}
