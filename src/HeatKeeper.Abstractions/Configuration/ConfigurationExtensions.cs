using System;
using Microsoft.Extensions.Configuration;

namespace HeatKeeper.Abstractions.Configuration;


public static class ConfigurationExtensions
{
    public static string GetInfluxDbUrl(this IConfiguration configuration)
        => configuration.IsRunningInContainer() ? "http://influxdb:8086" : "http://localhost:8086";

    internal static bool IsRunningInContainer(this IConfiguration configuration) => configuration.GetValue("DOTNET_RUNNING_IN_CONTAINER", false);

    public static string GetInfluxDbApiKey(this IConfiguration configuration)
        => configuration.GetRequiredValue("INFLUXDB_API_KEY");

    public static string GetEntsoeApiKey(this IConfiguration configuration)
        => configuration.GetRequiredValue("ENTSOE_SECURITY_TOKEN");

    public static string GetInfluxDbOrganization(this IConfiguration configuration)
        => configuration.GetRequiredValue("INFLUXDB_ORGANIZATION");

    public static string GetMqttBrokerAddress(this IConfiguration configuration)
        => configuration.GetRequiredValue("MQTT_BROKER_ADDRESS");

    public static string GetMqttBrokerPassword(this IConfiguration configuration)
        => configuration.GetRequiredValue("MQTT_BROKER_PASSWORD");
    public static string GetMqttBrokerUser(this IConfiguration configuration)
        => configuration.GetRequiredValue("MQTT_BROKER_USER");

    public static string GetChannelStateCronExpression(this IConfiguration configuration)
        => configuration.GetRequiredValue("CHANNELSTATE_CRONEXPRESSION");

    public static string GetImportEnergyPricesCronExpression(this IConfiguration configuration)
        => configuration.GetRequiredValue("IMPORT_ENERGY_PRICES_CRONEXPRESSION");

    public static string GetDeleteExpiredMeasurementsCronExpression(this IConfiguration configuration)
    => configuration.GetRequiredValue("DELETE_EXPIRED_MEASUREMENTS_CRONEXPRESSION");

    public static string GetConnectionString(this IConfiguration configuration)
        => configuration.GetRequiredValue("CONNECTIONSTRING");
    public static string GetSecret(this IConfiguration configuration)
        => configuration.GetRequiredValue("SECRET");

    public static string GetVapidSubject(this IConfiguration configuration)
        => configuration.GetRequiredValue("VAPIDSubject");

    public static string GetVapidPublicKey(this IConfiguration configuration)
        => configuration.GetRequiredValue("VAPIDPublicKey");

    public static string GetVapidPrivateKey(this IConfiguration configuration)
        => configuration.GetRequiredValue("VAPIDPrivateKey");

    internal static string GetRequiredValue(this IConfiguration configuration, string key)
    {
        if (configuration.GetValue<string>(key) is null)
        {
            throw new Exception($"Missing configuration for {key}");
        }
        return configuration.GetValue<string>(key);
    }
}