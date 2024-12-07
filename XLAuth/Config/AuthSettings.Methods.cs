using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using XLAuth.Extensions;

namespace XLAuth.Config;

/// <summary>
/// The auth settings class
/// </summary>
/// <seealso cref="IEquatable{AuthSettings}"/>
/// <seealso cref="IEquatable{object}"/>
[Serializable]
internal sealed partial class AuthSettings : IEquatable<AuthSettings>, IEquatable<object> {
  internal event EventHandler? ReloadTriggered;

  internal void NotifyReloadTriggered([CallerMemberName] string caller = "") {
    var eventArgs = new PropertyChangedEventArgs(caller);
    this.ReloadTriggered?.Invoke(this, eventArgs);
  }

  /// <summary>
  /// Test if one instance of <see cref="AuthSettings"/> is equal to the other.
  /// </summary>
  /// <param name="other">The other <see cref="AuthSettings"/>.</param>
  /// <returns>The bool</returns>
  public bool Equals(AuthSettings? other)
    => other is not null
       && this.CloseApp == other.CloseApp
       && this.CurrentAccountID.Equals(other.CurrentAccountID)
       && this.Language == other.Language
       && this.VersionUpgradeLevel == other.VersionUpgradeLevel
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
           && this.Equals((AuthSettings)obj)));

  /// <summary>
  /// Gets the hash code
  /// </summary>
  /// <returns>The int</returns>
  public override int GetHashCode() => base.GetHashCode();

  /// <summary>
  /// Uses the commandline arguments
  /// </summary>
  /// <returns>The auth settings</returns>
  private AuthSettings UseCommandlineArguments() {
    var type = typeof(AuthSettings);
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
  internal static AuthSettings Load(string path = "") {
    if (path.IsNullOrEmptyOrWhiteSpace()) {
      path = Path.Combine(Paths.RoamingPath, "authConfigV1.json");
    }

    if (!File.Exists(path)) {
        new AuthSettings().Save(path);
    }

    using var file = File.OpenText(path);
    var jsonText = file.ReadToEnd();

    return (JsonConvert.DeserializeObject<AuthSettings>(jsonText, Util.SerializerSettings) ?? new AuthSettings()).UseCommandlineArguments();
  }
}
