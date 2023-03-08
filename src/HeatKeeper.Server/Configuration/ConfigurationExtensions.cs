using System;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Server.Configuration;

public static class ConfigurationExtensions
{
    public static string GetInfluxDbUrl(this IConfiguration configuration)
        => configuration.IsRunningInContainer() ? "http://influxdb:8086" : "http://localhost:8086";

    internal static bool IsRunningInContainer(this IConfiguration configuration) => configuration.GetValue("DOTNET_RUNNING_IN_CONTAINER", false);

    public static string GetInfluxDbApiKey(this IConfiguration configuration)
        => configuration.GetRequiredValue("INFLUXDB_API_KEY");

    public static string GetInfluxDbOrganization(this IConfiguration configuration)
        => configuration.GetRequiredValue("INFLUXDB_ORGANIZATION");

    public static string GetMqttBrokerAddress(this IConfiguration configuration)
        => configuration.GetRequiredValue("MQTT_BROKER_ADDRESS");

    public static string GetMqttBrokerPassword(this IConfiguration configuration)
        => configuration.GetRequiredValue("MQTT_BROKER_PASSWORD");
    public static string GetMqttBrokerUser(this IConfiguration configuration)
    => configuration.GetRequiredValue("MQTT_BROKER_USER");

    internal static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        if (configuration.GetValue<string>(key) is null)
        {
            throw new Exception($"Missing configuration for {key}");
        }
        return configuration.GetValue<string>(key);
    }
}