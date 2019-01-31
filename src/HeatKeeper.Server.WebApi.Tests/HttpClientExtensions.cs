using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using HeatKeeper.Server.Host.Locations;
using HeatKeeper.Server.Host.Users;
using HeatKeeper.Server.Users;
using Newtonsoft.Json;

namespace HeatKeeper.Server.WebApi.Tests
{
    public static class HttpClientExtensions
    {
        public static async Task<T> GetAsync<T>(this HttpClient client, string requestUri)
        {
            var result = await client.GetAsync(requestUri);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<T>();
        }

        public static async Task<HttpResponseMessage> PostAsync<T>(this HttpClient client, string requestUri, string bearerToken, T content)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(requestUri),
                Content = new JsonContent(content)
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            return await client.SendAsync(request);
        }

        public static async Task<T> ContentAs<T>(this HttpResponseMessage response)
        {
            var data = await response.Content.ReadAsStringAsync();
            return string.IsNullOrEmpty(data) ?
                            default(T) :
                            JsonConvert.DeserializeObject<T>(data);
        }


        public static async Task<string> AuthenticateAsAdminUser(this HttpClient client)
        {
            var request = new AuthenticateUserRequest(AdminUser.UserName, AdminUser.DefaultPassword);
            var response = await client.PostAsync("api/users/authenticate", new JsonContent(request));
            var content = await response.ContentAs<AuthenticateUserResponse>();
            return content.Token;
        }

        public static async Task<HttpResponseMessage> CreateLocation(this HttpClient client, CreateLocationRequest request)
        {
            var token = await client.AuthenticateAsAdminUser();
            var httpRequest = new HttpRequestBuilder()
                .AddBearerToken(token)
                .AddContent(new JsonContent(request))
                .AddMethod(HttpMethod.Post)
                .AddRequestUri("api/locations")
                .Build();
            return  await client.SendAsync(httpRequest);
        }
    }
}