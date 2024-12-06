using Newtonsoft.Json;

namespace XLAuthenticatorNet.Models.Abstracts;

/// <summary>
/// The ixl launcher response interface
/// </summary>
public interface IXLLauncherResponse {
  /// <summary>
  /// Gets or sets the value of the version
  /// </summary>
  [JsonProperty("version")]
  public string Version { get; set; }
  /// <summary>
  /// Gets or sets the value of the app
  /// </summary>
  [JsonProperty("app")]
  public string App { get; set; }
}
