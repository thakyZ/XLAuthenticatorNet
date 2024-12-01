using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Serilog.Events;
using XLAuthenticatorNet.Config.Converters;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Support;

namespace XLAuthenticatorNet.Config;

/// <summary>
/// The auth settings class
/// </summary>
/// <seealso cref="IEquatable{AuthSettingsV1}"/>
[Serializable]
[SuppressMessage("Naming", "AV1704:Identifier contains one or more digits in its name")]
internal sealed class AuthSettingsV1 : IEquatable<AuthSettingsV1> {
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
    var eventArgs = new PropertyChangedEventArgs(caller);
    this.ReloadTriggered?.Invoke(this, eventArgs);
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
       && string.Equals(this.AcceptLanguage, other.AcceptLanguage, StringComparison.Ordinal)
       && this.LogLevel == other.LogLevel;

  /// <summary>
  /// Equalses the obj
  /// </summary>
  /// <param name="obj">The obj</param>
  /// <returns>The bool</returns>
  public override bool Equals(object? obj)
    => obj is not null
       && (ReferenceEquals(this, obj)
           || (obj.GetType() == this.GetType()
           && this.Equals((AuthSettingsV1)obj)));

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
    var type = typeof(AuthSettingsV1);
    PropertyInfo[] properties = type.GetProperties();
    var cliArgs = Environment.GetCommandLineArgs();
    List<ArgumentPair> arguments = ArgumentParser.ParseArguments(cliArgs);
    foreach (var property in properties) {
      List<Attribute> attributes = property.GetCustomAttributes().ToList();
      if (attributes.Find(attribute => attribute is JsonPropertyAttribute) is not JsonPropertyAttribute propertyAttribute) {
        continue;
      }

      List<ArgumentPair> argumentValues = arguments.Where(pair => pair.Name.Equals(propertyAttribute.PropertyName, StringComparison.OrdinalIgnoreCase)).ToList();
      if (argumentValues.Count < 1 || !argumentValues.TrueForAll(pair => pair.Value is not null)) {
        continue;
      }

      if ((property.PropertyType.IsGenericType && property.PropertyType.IsEquivalentTo(typeof(List<>))) || property.PropertyType.IsEquivalentTo(typeof(Array))) {
        var value = arguments.WhereIn(property.PropertyType.GenericTypeArguments, (pair, genericType) => pair.Value!.GetType().IsEquivalentTo(genericType)).Select(pair => pair.Value).ToArray();
        property.SetValue(this, value);
      } else {
        var argumentValueType = argumentValues[0].Value!.GetType();
        if (property.PropertyType.IsEquivalentTo(argumentValueType)) {
          property.SetValue(this, argumentValues[0].Value);
        }
      }
    }

    return this;
  }

  /// <summary>
  /// Saves the path
  /// </summary>
  /// <param name="path">The path</param>
  internal void Save(string path = "") {
    if (path.IsNullOrEmptyOrWhiteSpace()) {
      path = Path.Combine(Paths.RoamingPath, "authConfigV1.json");
    }
    using var file = File.OpenWrite(path);
    using var writer = new StreamWriter(file);
    var text = JsonConvert.SerializeObject(this, Formatting.Indented);
    writer.Write(text);
  }

  /// <summary>
  /// Loads the path
  /// </summary>
  /// <param name="path">The path</param>
  /// <returns>The output</returns>
  internal static AuthSettingsV1 Load(string path = "") {
    if (path.IsNullOrEmptyOrWhiteSpace()) {
      path = Path.Combine(Paths.RoamingPath, "authConfigV1.json");
    }

    if (!File.Exists(path)) {
        new AuthSettingsV1().Save(path);
    }

    using var file = File.OpenText(path);
    var jsonText = file.ReadToEnd();
    var output = (JsonConvert.DeserializeObject<AuthSettingsV1>(jsonText, App.SerializerSettings) ?? new AuthSettingsV1()).UseCommandlineArguments();

    if (string.IsNullOrEmpty(output.AcceptLanguage)) {
      output.AcceptLanguage = ApiHelpers.GenerateAcceptLanguage();
    }

    return output;
  }
}