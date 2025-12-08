using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace HeatKeeper.Server.Host;


public static class OAuth2
{
    static ConcurrentDictionary<string, string> authCodes = new ConcurrentDictionary<string, string>();

    // accessToken -> userId
    static ConcurrentDictionary<string, string> accessTokens = new ConcurrentDictionary<string, string>();

    // Configure your Google client + redirect URI (must match what's in Google console)
    const string GoogleClientId = "heatkeeper_google_home";
    const string GoogleClientSecret = "heatkeeper_client_secret";
    const string GoogleRedirectUri = "https://oauth-redirect.googleusercontent.com/r/heatkeeperintegration-94284";

    // For dev we just have a single test user
    const string TestUserId = "user-123";


    public static void MapAuthorizeEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/oauth2/authorize", async (HttpContext ctx) =>
        {
            var query = ctx.Request.Query;

            var clientId = query["client_id"].ToString();
            var redirectUri = query["redirect_uri"].ToString();
            var responseType = query["response_type"].ToString();
            var scope = query["scope"].ToString();
            var state = query["state"].ToString();

            // Basic validation
            if (clientId != GoogleClientId ||
                redirectUri != GoogleRedirectUri ||
                responseType != "code")
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("Invalid OAuth request");
                return;
            }

            // In real life: check if user is logged in, show login/consent UI, etc.
            // For dev: assume the user is already authenticated as TestUserId.

            var userId = TestUserId;

            // Generate a one-time authorization code
            var code = Guid.NewGuid().ToString("N");
            authCodes[code] = userId;

            // Redirect back to Google with code + state
            var redirectParams = new Dictionary<string, string?>
            {
                ["code"] = code,
                ["state"] = state
            };

            var redirectUrl = QueryHelpers.AddQueryString(redirectUri, redirectParams);

            ctx.Response.Redirect(redirectUrl);
        });



        //  /oauth2/token
        // =======================
        //  /oauth2/token
        // =======================
        endpoints.MapPost("/api/oauth2/token", async (HttpContext ctx) =>
        {
            if (!ctx.Request.HasFormContentType)
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("Expected form-encoded body");
                return;
            }

            var form = await ctx.Request.ReadFormAsync();
            var grantType = form["grant_type"].ToString();
            var code = form["code"].ToString();
            var redirectUri = form["redirect_uri"].ToString();

            // Try to get credentials from form body first
            var clientId = form["client_id"].ToString();
            var clientSecret = form["client_secret"].ToString();

            // If not in form body, try HTTP Basic Auth header
            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                var authHeader = ctx.Request.Headers.Authorization.ToString();
                if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var encodedCredentials = authHeader.Substring("Basic ".Length).Trim();
                        var decodedCredentials = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(encodedCredentials));
                        var parts = decodedCredentials.Split(':', 2);

                        if (parts.Length == 2)
                        {
                            clientId = parts[0];
                            clientSecret = parts[1];
                        }
                    }
                    catch
                    {
                        ctx.Response.StatusCode = 400;
                        await ctx.Response.WriteAsync("Invalid Basic Auth header");
                        return;
                    }
                }
            }

            // Validate grant type
            if (grantType != "authorization_code")
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync($"Unsupported grant_type: {grantType}");
                return;
            }

            // Validate client credentials
            if (clientId != GoogleClientId || clientSecret != GoogleClientSecret)
            {
                ctx.Response.StatusCode = 401;
                await ctx.Response.WriteAsync($"Invalid client credentials. ClientId: '{clientId}', ClientSecret: '{clientSecret}'");
                return;
            }

            // Validate redirect URI
            if (redirectUri != GoogleRedirectUri)
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync($"Invalid redirect_uri: {redirectUri}");
                return;
            }

            // Validate and consume authorization code
            if (!authCodes.TryRemove(code, out var userId))
            {
                ctx.Response.StatusCode = 400;
                await ctx.Response.WriteAsync("Invalid or expired authorization code");
                return;
            }

            // Issue an access token (and optionally a refresh token)
            var accessToken = Guid.NewGuid().ToString("N");
            accessTokens[accessToken] = userId;

            var responseObj = new
            {
                access_token = accessToken,
                token_type = "Bearer",
                expires_in = 3600,
                scope = form["scope"].ToString(), // optional
                refresh_token = "" // you can skip refresh for dev
            };

            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsync(JsonSerializer.Serialize(responseObj));
        });
        endpoints.MapPost("/api/fulfillment", async (HttpContext ctx) =>
{
    // OPTIONAL: validate bearer token from Google
    // For dev you can skip this completely if you only have one user,
    // but here's how you'd do it:

    string? userId = null;

    var authHeader = ctx.Request.Headers.Authorization.ToString();
    if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
    {
        var token = authHeader.Substring("Bearer ".Length).Trim();
        accessTokens.TryGetValue(token, out userId);
    }

    // For dev, if we didn't find userId, just fall back to test user
    userId ??= TestUserId;

    using var doc = await JsonDocument.ParseAsync(ctx.Request.Body);
    var root = doc.RootElement;

    var requestId = root.GetProperty("requestId").GetString();
    var inputs = root.GetProperty("inputs");
    var intent = inputs[0].GetProperty("intent").GetString();

    switch (intent)
    {
        case "action.devices.SYNC":
            {
                // You can use userId here if needed (per-user devices)
                var response = new
                {
                    requestId,
                    payload = new
                    {
                        agentUserId = userId,
                        devices = new[]
                        {
                       new
                        {
                            id = "living-room-sensor",
                            type = "action.devices.types.SENSOR",
                            traits = new[]
                            {
                                "action.devices.traits.TemperatureSetting",  // For reading temperature
                                "action.devices.traits.HumiditySetting"       // For reading humidity
                            },
                            name = new
                            {
                                defaultNames = new[] { "Living Room Sensor" },
                                name = "Living room sensor",
                                nicknames = new[] { "Living room" }
                            },
                            willReportState = false,
                            attributes = new
                            {
                                queryOnlyTemperatureSetting = true,  // READ-ONLY temperature
                                queryOnlyHumiditySetting = true      // READ-ONLY humidity
                            },
                            roomHint = "Living room"
                        }
                    }
                    }
                };

                return Results.Json(response);
            }

        case "action.devices.QUERY":
            {
                var payload = inputs[0].GetProperty("payload");
                var devices = payload.GetProperty("devices").EnumerateArray();

                var resultDevices = new Dictionary<string, object>();

                foreach (var d in devices)
                {
                    var id = d.GetProperty("id").GetString();

                    if (id == "living-room-sensor")
                    {
                        // TODO: replace with real HeatKeeper lookup for this user
                        var temp = GetLivingRoomTemperatureFromHeatKeeper(userId);

                        resultDevices[id!] = new
                        {
                            online = true,
                            status = "SUCCESS",
                            thermostatTemperatureAmbient = temp
                        };
                    }
                }

                return Results.Json(new
                {
                    requestId,
                    payload = new
                    {
                        devices = resultDevices
                    }
                });
            }

        default:
            return Results.BadRequest(new { error = "Unknown intent" });
    }
});
    }

    private static object GetLivingRoomTemperatureFromHeatKeeper(string userId) => 22.7;
}