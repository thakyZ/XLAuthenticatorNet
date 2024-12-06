using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace XLAuth.Update;

internal sealed partial class Updates {
  /// <summary>
  /// The error news data class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  internal sealed class ErrorNewsData {
    /// <summary>
    /// Gets or sets the value of the show until
    /// </summary>
    [JsonPropertyName("until")]
    internal uint ShowUntil { get; set; }

    /// <summary>
    /// Gets or sets the value of the message
    /// </summary>
    [JsonPropertyName("message")]
    internal required string Message { get; set; }

    /// <summary>
    /// Gets or sets the value of the is error
    /// </summary>
    [JsonPropertyName("isError")]
    internal bool IsError { get; set; }
  }
}
