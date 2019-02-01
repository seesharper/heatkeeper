namespace HeatKeeper.Server.Host.Users
{
    public class GetApiKeyResponse
    {
        public GetApiKeyResponse(string apiKey)
        {
            ApiKey = apiKey;
        }

        public string ApiKey { get; }
    }
}