using System.CodeDom.Compiler;
using System.Net;
using Newtonsoft.Json;
using XLAuth.Config.Converters;

namespace XLAuth.Config;
internal sealed partial class TOTPAccount {
  /// <summary>
  /// Gets the value of the account id
  /// </summary>
  [JsonProperty("id"), JsonConverter(typeof(GuidJsonConverter))]
  public Guid Id { get; }

  /// <summary>
  /// Gets or sets the value of the account name
  /// </summary>
  [JsonProperty("name")]
  public string Name { get; set; } = null!;

  /// <summary>
  /// Gets or sets the value of the launcher IP address
  /// </summary>
  [JsonProperty("address"), JsonConverter(typeof(IPAddressJsonConverter))]
  public IPAddress? LauncherIpAddress { get; set; } = null;
}
