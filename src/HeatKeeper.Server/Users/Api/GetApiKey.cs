using HeatKeeper.Server.Authentication;

namespace HeatKeeper.Server.Users.Api;

public class GetApiKey(IApiKeyProvider apiKeyProvider) : IQueryHandler<ApiKeyQuery, ApiKey>
{
    public Task<ApiKey> HandleAsync(ApiKeyQuery query, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(apiKeyProvider.CreateApiKey());
    }
}

[RequireAdminRole]
[Get("/api/users/apikey")]
public record ApiKeyQuery : IQuery<ApiKey>;

