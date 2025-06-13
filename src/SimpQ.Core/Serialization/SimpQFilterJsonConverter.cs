using System.Text.Json.Serialization;
using System.Text.Json;
using SimpQ.Abstractions.Models.Requests;

namespace SimpQ.Core.Serialization;

/// <summary>
/// A custom JSON converter for the <see cref="IFilter"/> base type that supports polymorphic deserialization
/// of either a <see cref="FilterCondition"/> or a <see cref="FilterGroup"/>, depending on the presence of a <c>conditions</c> property.
/// </summary>
public class SimpQFilterJsonConverter : JsonConverter<IFilter> {
    /// <summary>
    /// Reads and deserializes JSON into either a <see cref="FilterGroup"/> or <see cref="FilterCondition"/> 
    /// based on the structure of the incoming JSON object.
    /// </summary>
    /// <param name="reader">The reader to read from.</param>
    /// <param name="typeToConvert">The type of object to convert.</param>
    /// <param name="options">Serialization options to use when deserializing nested types.</param>
    /// <returns>
    /// A <see cref="IFilter"/> instance, which can be either a <see cref="FilterGroup"/> or a <see cref="FilterCondition"/>, 
    /// or <c>null</c> if deserialization fails.
    /// </returns>
    public override IFilter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (root.TryGetProperty("conditions", out _))
            return JsonSerializer.Deserialize<FilterGroup>(root.GetRawText(), options);
        return JsonSerializer.Deserialize<FilterCondition>(root.GetRawText(), options);
    }

    /// <summary>
    /// Writes the <see cref="IFilter"/> object to JSON using the default serializer.
    /// </summary>
    /// <param name="writer">The writer to write to.</param>
    /// <param name="value">The <see cref="IFilter"/> object to serialize.</param>
    /// <param name="options">Serialization options to use.</param>
    public override void Write(Utf8JsonWriter writer, IFilter value, JsonSerializerOptions options) =>
        JsonSerializer.Serialize(writer, (object)value, options);
}