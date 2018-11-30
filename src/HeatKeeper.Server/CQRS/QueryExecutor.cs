using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using LightInject;

namespace HeatKeeper.Server.CQRS
{
    /// <summary>
    /// An <see cref="IQueryExecutor"/> that is capable of executing a query.
    /// </summary>
    public class QueryExecutor : IQueryExecutor
    {
        private static readonly MethodInfo GetInstanceMethod;
        private static readonly MethodInfo GetTypeFromHandleMethod;
        private readonly IServiceFactory _factory;

        static QueryExecutor()
        {
            GetInstanceMethod = typeof(IServiceFactory).GetMethod("GetInstance", new[] { typeof(Type) });
            GetTypeFromHandleMethod = typeof(Type).GetMethod("GetTypeFromHandle", BindingFlags.Public | BindingFlags.Static);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryExecutor"/> class.
        /// </summary>
        /// <param name="factory">The <see cref="IServiceFactory"/> used to resolve the
        /// <see cref="IQueryHandler{TQuery,TResult}"/> to be executed.</param>
        public QueryExecutor(IServiceFactory factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Executes the given <paramref name="query"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of result returned by the query.</typeparam>
        /// <param name="query">The query to be executed.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The result from the query.</returns>
        public async Task<TResult> ExecuteAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken)
        {
            var queryDelegate = Cache<TResult>.GetOrAdd(query.GetType(), CreateDelegate<TResult>);
            return await queryDelegate(_factory, query, cancellationToken);
        }

        private static Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>> CreateDelegate<TResult>(Type queryType)
        {
            // Define the signature of the dynamic method.
            var dynamicMethod = new System.Reflection.Emit.DynamicMethod("DynamicMethod", typeof(Task<TResult>), new[] { typeof(IServiceFactory), typeof(IQuery<TResult>) , typeof(CancellationToken) });
            System.Reflection.Emit.ILGenerator generator = dynamicMethod.GetILGenerator();

            // Create the closed generic query handler type.
            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResult));

            // Get the MethodInfo that represents the HandleAsync method.
            MethodInfo method = queryHandlerType.GetMethod("HandleAsync");

            // Push the service factory onto the evaluation stack.
            generator.Emit(OpCodes.Ldarg_0);

            // Push the query handler type onto the evaluation stack.
            generator.Emit(OpCodes.Ldtoken, queryHandlerType);
            generator.Emit(OpCodes.Call, GetTypeFromHandleMethod);

            // Call the GetInstance method and push the query handler
            // instance onto the evaluation stack.
            generator.Emit(OpCodes.Callvirt, GetInstanceMethod);

            // Since the GetInstance method returns an object,
            // we need to cast it to the actual query handler type.
            generator.Emit(OpCodes.Castclass, queryHandlerType);

            // Push the query onto the evaluation stack.
            generator.Emit(OpCodes.Ldarg_1);
            
            // The query is passed in as an IQuery<TResult> instance
            // and we need to cast it to the actual query type.
            generator.Emit(OpCodes.Castclass, queryType);

            //Push the cancellationtoken onto the evaluation stack.
            generator.Emit(OpCodes.Ldarg_2);

            // Call the HandleAsync method and push the Task<TResult>
            // onto the evaluation stack.
            generator.Emit(OpCodes.Callvirt, method);

            // Mark the end of the dynamic method.
            generator.Emit(OpCodes.Ret);

            var getQueryHandlerDelegate =
                dynamicMethod.CreateDelegate(typeof(Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>>));

            return (Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>>)getQueryHandlerDelegate;
        }

        private static class Cache<TResult>
        {
            private static ImmutableHashTree<Type, Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>>> hashTree =
                ImmutableHashTree<Type, Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>>>.Empty;

            public static Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>> GetOrAdd(Type queryType, Func<Type, Func<IServiceFactory, IQuery<TResult>, CancellationToken, Task<TResult>>> delegateFactory)
            {
                var func = hashTree.Search(queryType);
                if (func == null)
                {
                    func = delegateFactory(queryType);
                    Interlocked.Exchange(ref hashTree, hashTree.Add(queryType, func));
                }

                return func;
            }
        }
    }
}