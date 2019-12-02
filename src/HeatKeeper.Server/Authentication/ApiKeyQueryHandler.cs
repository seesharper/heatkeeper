using System.Threading;
using System.Threading.Tasks;
using CQRS.Query.Abstractions;
using HeatKeeper.Server.Authorization;

namespace HeatKeeper.Server.Authentication
{
    public class ApiKeyQueryHandler : IQueryHandler<ApiKeyQuery, ApiKey>
    {
        private readonly IApiKeyProvider apiKeyProvider;

        public ApiKeyQueryHandler(IApiKeyProvider apiKeyProvider)
        {
            this.apiKeyProvider = apiKeyProvider;
        }

        public Task<ApiKey> HandleAsync(ApiKeyQuery query, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(apiKeyProvider.CreateApiKey());
        }
    }

    [RequireAdminRole]
    public class ApiKeyQuery : IQuery<ApiKey>
    {
    }
}
