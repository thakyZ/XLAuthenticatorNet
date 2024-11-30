using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Serilog.Events;
using XLAuthenticatorNet.Config.Converters;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Support;

namespace XLAuthenticatorNet.Config;

/// <summary>
/// The auth settings class
/// </summary>
/// <seealso cref="IEquatable{AuthSettingsV1}"/>
[Serializable]
internal class AuthSettingsV1 : IEquatable<AuthSettingsV1> {
#region Program Settings

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
  /// Gets or sets the value of the accept language
  /// </summary>
  [JsonProperty("accept_language")]
  internal string AcceptLanguage { get; set; } = ((Language?)null).GetLocalizationCode();

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
#endregion Program Settings

  internal event EventHandler? ReloadTriggered;

  internal void NotifyReloadTriggered([CallerMemberName] string caller = "") {
    this.ReloadTriggered?.Invoke(caller, EventArgs.Empty);
  }

  /// <summary>
  /// Test if one instance of <see cref="AuthSettingsV1"/> is equal to the other.
  /// </summary>
  /// <param name="other">The other <see cref="AuthSettingsV1"/>.</param>
  /// <returns>The bool</returns>
  public bool Equals(AuthSettingsV1? other)
    => other is not null
       && this.CloseApp == other.CloseApp
       && this.CurrentAccountID.Equals(other.CurrentAccountID)
       && this.Language == other.Language
       && this.VersionUpgradeLevel == other.VersionUpgradeLevel
       && this.AcceptLanguage == other.AcceptLanguage
       && this.LogLevel == other.LogLevel;

  /// <summary>
  /// Equalses the obj
  /// </summary>
  /// <param name="obj">The obj</param>
  /// <returns>The bool</returns>
  public override bool Equals(object? obj)
    => obj is not null
       && (ReferenceEquals(this, obj)
           || obj.GetType() == GetType()
           && Equals((AuthSettingsV1)obj));

  /// <summary>
  /// Gets the hash code
  /// </summary>
  /// <returns>The int</returns>
  [SuppressMessage("ReSharper", "BaseObjectGetHashCodeCallInGetHashCode")]
  public override int GetHashCode() => base.GetHashCode();

  /// <summary>
  /// Uses the commandline arguments
  /// </summary>
  /// <returns>The auth settings</returns>
  private AuthSettingsV1 UseCommandlineArguments() {
    Type t = typeof(AuthSettingsV1);
    PropertyInfo[] properties = t.GetProperties();
    List<ArgumentPair> arguments = ArgumentParser.ParseArguments(Environment.GetCommandLineArgs());
    foreach (PropertyInfo property in properties) {
      var attributes = property.GetCustomAttributes().ToList();
      if (attributes.FirstOrDefault(x => x is JsonPropertyAttribute) is not JsonPropertyAttribute propertyAttribute)
        continue;
      var argumentValues = arguments.Where(x => x.Name == propertyAttribute.PropertyName).ToList();
      if (argumentValues.Count < 1 || !argumentValues.All(x => x.Value is not null))
        continue;
      if ((property.PropertyType.IsGenericType && property.PropertyType.IsEquivalentTo(typeof(List<>))) || property.PropertyType.IsEquivalentTo(typeof(Array))) {
        property.SetValue(this, arguments.Where(x => property.PropertyType.GenericTypeArguments.All(y => x.Value!.GetType().IsEquivalentTo(y))).Select(x => x.Value).ToArray());
      } else if (property.PropertyType.IsEquivalentTo(argumentValues.First().Value!.GetType())) {
        property.SetValue(this, argumentValues.First().Value!);
      }
    }

    return this;
  }

  /// <summary>
  /// Saves the path
  /// </summary>
  /// <param name="path">The path</param>
  internal void Save(string? path = null) {
    path ??= Path.Combine(Paths.RoamingPath, "authConfigV1.json");
    using FileStream file = File.OpenWrite(path);
    using var writer = new StreamWriter(file);
    writer.Write(JsonConvert.SerializeObject(this, Formatting.Indented));
  }

  /// <summary>
  /// Loads the path
  /// </summary>
  /// <param name="path">The path</param>
  /// <returns>The output</returns>
  internal static AuthSettingsV1 Load(string? path = null) {
    path ??= Path.Combine(Paths.RoamingPath, "authConfigV1.json");
    if (!File.Exists(path)) {
        new AuthSettingsV1().Save(path);
    }
    using StreamReader file = File.OpenText(path);
    string jsonText = file.ReadToEnd();
    AuthSettingsV1 output = (JsonConvert.DeserializeObject<AuthSettingsV1>(jsonText, App.SerializerSettings) ?? new AuthSettingsV1()).UseCommandlineArguments();

    if (string.IsNullOrEmpty(output.AcceptLanguage)) {
      output.AcceptLanguage = ApiHelpers.GenerateAcceptLanguage();
    }

    return output;
  }
}