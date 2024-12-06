using System;
using System.Net;
using Newtonsoft.Json;

namespace XLAuthenticatorNet.Config.Converters;

/// <summary>
/// JSON converter for <see cref="TSource:System.Guid"/>.
/// <seealso href="https://gist.github.com/ReubenBond/9416e7a4c528eab473fd"/>
/// </summary>
internal sealed class IPAddressJsonConverter : JsonConverter<IPAddress> {
  /// <summary>
  /// Gets a value indicating whether this <see cref="TSource:Newtonsoft.Json.JsonConverter"/> can write JSON.
  /// </summary>
  /// <value><see langword="true"/> if this <see cref="TSource:Newtonsoft.Json.JsonConverter}"/> can write JSON; otherwise, <see langword="false"/>.</value>
  public override bool CanWrite => true;

  /// <summary>Writes the JSON representation of the object.</summary>
  /// <param name="writer">The <see cref="TSource:Newtonsoft.Json.JsonWriter"/> to write to.</param>
  /// <param name="value">The value.</param>
  /// <param name="serializer">The calling serializer.</param>
  public override void WriteJson(JsonWriter writer, IPAddress? value, JsonSerializer serializer) {
    if (value is null) {
      writer.WriteNull();
      return;
    }

    writer.WriteValue(value.ToString());
  }

  /// <summary>
  /// Reads the JSON representation of the object.
  /// </summary>
  /// <param name="reader">The <see cref="TSource:Newtonsoft.Json.JsonReader"/> to read from.</param>
  /// <param name="objectType">Kind of the object.</param>
  /// <param name="existingValue">The existing value of object being read.</param>
  /// <param name="hasExistingValue">Determines if there is an existing value.</param>
  /// <param name="serializer">The calling serializer.</param>
  /// <returns>The object value.</returns>
  public override IPAddress? ReadJson(JsonReader reader, Type objectType, IPAddress? existingValue, bool hasExistingValue, JsonSerializer serializer) {
    return reader.Value is string str && IPAddress.TryParse(str, out var address) ? address : null;
  }
}
