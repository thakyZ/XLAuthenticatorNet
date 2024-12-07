using Newtonsoft.Json;
using Serilog.Events;
using XLAuth.Config.Converters;

namespace XLAuth.Config;
internal sealed partial class AuthSettings {
  /// <summary>
  /// Gets or sets the value of the close app
  /// </summary>
  [JsonProperty("close_app")]
  internal bool CloseApp { get; set; }

  /// <summary>
  /// Gets or sets the value of the current account id
  /// </summary>
  [JsonProperty("current_account"), JsonConverter(typeof(GuidJsonConverter))]
  internal Guid CurrentAccountID { get; set; } = Guid.Empty;

  /// <summary>
  /// Gets or sets the value of the language
  /// </summary>
  [JsonProperty("language")]
  internal Language? Language { get; set; }

  /// <summary>
  /// Gets or sets the value of the version upgrade level
  /// </summary>
  [JsonProperty("version_upgrade_level")]
  internal int VersionUpgradeLevel { get; set; }

  /// <summary>
  /// Gets or sets the value of the log level
  /// </summary>
  [JsonProperty("log_level")]
  internal LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;

  /// <summary>
  /// Gets or sets the value of the unique id cache enabled
  /// </summary>
  [JsonProperty("unique_id_cache_enabled")]
  internal bool UniqueIdCacheEnabled { get; set; }
}
