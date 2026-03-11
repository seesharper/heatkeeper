using System.Text.Json;
using System.Text.Json.Serialization;

namespace HeatKeeper.Server.Events;

/// <summary>
/// Converts enum values from JSON strings containing either the enum name ("Active") or its
/// numeric value ("1"), as produced when action parameters are passed as string values.
/// </summary>
public sealed class EnumFromStringConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert) => typeToConvert.IsEnum;

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        var converterType = typeof(EnumFromStringConverterInner<>).MakeGenericType(typeToConvert);
        return (JsonConverter)Activator.CreateInstance(converterType)!;
    }

    private sealed class EnumFromStringConverterInner<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str == null)
                throw new JsonException($"Cannot convert null to {typeToConvert.Name}.");

            // Try parsing as integer first ("1"), then by name ("Active")
            if (int.TryParse(str, out var intValue))
                return (T)Enum.ToObject(typeToConvert, intValue);

            if (Enum.TryParse<T>(str, ignoreCase: true, out var result))
                return result;

            throw new JsonException($"Cannot convert \"{str}\" to {typeToConvert.Name}.");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString());
    }
}
