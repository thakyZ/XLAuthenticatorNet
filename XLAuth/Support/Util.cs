using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using XLAuth.Extensions;
using XLAuth.Windows;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using Env = DotEnv.Generated.Environment;

namespace XLAuth.Support;

/// <summary>
/// The application utility class
/// </summary>
internal static class Util {
  /// <summary>
  /// Finds the child by name using the specified parent <see cref="DependencyObject" /> and searching for a <see cref="System.Windows.Controls.Control" /> by name.
  /// </summary>
  /// <param name="parent">The parent <see cref="DependencyObject" /> to query.</param>
  /// <param name="controlName">The name of the <see cref="System.Windows.Controls.Control" />.</param>
  /// <returns>The <see cref="DependencyObject" /> if found otherwise null.</returns>
  internal static DependencyObject? FindChildByName(DependencyObject parent, string controlName) {
    int count = VisualTreeHelper.GetChildrenCount(parent);

    for (var index = 0; index < count; index++) {
      DependencyObject myChild = VisualTreeHelper.GetChild(parent, index);

      if (myChild is FrameworkElement element && element.Name?.Equals(controlName, StringComparison.OrdinalIgnoreCase) == true) {
        return element;
      }

      DependencyObject? findResult = Util.FindChildByName(myChild, controlName);

      if (findResult is not null) {
        return findResult;
      }
    }

    return null;
  }

  /// <summary>
  /// Gets an instance of a <see cref="Style" /> given the specified key.
  /// </summary>
  /// <param name="key">The key of the <see cref="Style" /> resource.</param>
  /// <exception cref="Exception">Thrown when <see cref="Application.Current" /> is not typeof <see cref="XLAuth.App" />.</exception>
  /// <exception cref="Exception">Thrown when failed to get <see cref="Style" />.</exception>
  /// <returns>The <see cref="Style" /> that was obtained.</returns>
  internal static Style GetStyle(string key) {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    if (app.FindResource(key) is Style style) {
      return style;
    }

    throw new Exception($"Could not find style with key, \"{key}\".");
  }

  /// <summary>
  /// Gets an instance of a <see cref="Brush" /> given the specified key and optionally the specified service.
  /// </summary>
  /// <param name="key">The key of the <see cref="Brush" /> resource.</param>
  /// <param name="type">The type of service.</param>
  /// <exception cref="Exception">Thrown when <see cref="Application.Current" /> is not typeof <see cref="XLAuth.App" />.</exception>
  /// <exception cref="Exception">Thrown when failed to get <see cref="Brush" />.</exception>
  /// <returns>The <see cref="Brush" /> that was obtained.</returns>
  internal static Brush GetBrush(string key, Type? type = null) {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    Brush? output = null;

    if (app.TryFindResource(key) is Brush brush1) {
      output = brush1;
    } else if (app.Resources.FindName(key) is Brush brush2) {
      output = brush2;
    } else if (app.Resources.MergedDictionaries.Where(resourceDictionary => resourceDictionary.FindName(key) is not null).Select(resourceDictionary => resourceDictionary.FindName(key)).FirstOrDefault() is Brush brush3) {
      output = brush3;
    } else if (new DynamicResourceExtension(key).ProvideValue(serviceProvider: null) is Brush brush4) {
      output = brush4;
    } else if (type is not null && new ServiceProviders().GetService(type) is IServiceProvider provider && new StaticResourceExtension(key).ProvideValue(provider) is Brush brush5) {
      output = brush5;
    }

#if DEBUG
    foreach (KeyValuePair<object, object?> brushKey in app.Resources.MergedDictionaries.SelectMany(resourceDictionary => resourceDictionary.ToList()).Where(keyValuePair => keyValuePair.Value is Brush)) {
      Logger.Debug("brushKey.Key:   {0}", brushKey.Key);
      Logger.Debug("brushKey.Value: {0}", brushKey.Value);
    }
#endif

    return output ?? throw new Exception($"Could not find brush with key, \"{key}\".");
  }

  /*
  /// <summary>
  /// Gets an instance of a <see cref="Brush" /> given the specified key and optionally the specified service.
  /// </summary>
  /// <param name="key">The key of the <see cref="Brush" /> resource.</param>
  /// <param name="type">The type of service.</param>
  /// <exception cref="Exception">Thrown when <see cref="Application.Current" /> is not typeof <see cref="XLAuth.App" />.</exception>
  /// <exception cref="Exception">Thrown when could not find brush with specified <paramref name="key" />.</exception>
  /// <exception cref="Exception">Thrown when could not find resource with specified <paramref name="assembly" />, <paramref name="color" /> and <paramref name="isPrimary" />.</exception>
  /// <exception cref="Exception">Thrown when failed to get <see cref="Brush" />.</exception>
  /// <returns>The <see cref="Brush" /> that was obtained.</returns>
  private static Brush GetMaterialDesignBrush(string assembly, string color, bool isPrimary, string key) {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    var brushUrl = string.Format(CultureInfo.InvariantCulture, "/{0};component/Themes/{0}.{1}.{2}.xaml", assembly, color, isPrimary ? "Primary" : "Secondary");
    var uri = new Uri(brushUrl, UriKind.RelativeOrAbsolute);
    ResourceDictionary? output;

    if (app.Resources.MergedDictionaries.All((ResourceDictionary resourceDictionary) => resourceDictionary.Source != uri)) {
      output = new ResourceDictionary {
        Source = uri,
      };

      app.Resources.MergedDictionaries.Add(output);
    } else {
      output = app.Resources.MergedDictionaries.FirstOrDefault((ResourceDictionary resourceDictionary) => resourceDictionary.Source == uri);
    }

    if (output is null) {
      throw new Exception($"Could not find resource with uri, \"{uri}\".");
    }

    if (output.FindName(key) is Brush brush) {
      return brush;
    }

    throw new Exception($"Could not find brush with key, \"{key}\".");
  }
  */

  /// <summary>
  /// Gets an instance of the current <see cref="Application" />'s <see cref="Theme" />.
  /// </summary>
  /// <exception cref="Exception">Thrown when <see cref="Application.Current" /> is not typeof <see cref="XLAuth.App" />.</exception>
  /// <returns>The <see cref="Theme" /> that was obtained.</returns>
  internal static Theme? GetTheme() {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    try {
      return app.Resources.GetTheme();
    } catch (Exception exception) {
      Logger.Error(exception, "Failed to get theme.");

      return null;
    }
  }

  /// <summary>
  /// Gets the global value of the <see cref="JsonSerializerSettings" />.
  /// </summary>
  internal static JsonSerializerSettings SerializerSettings { get; } = new() {
    Formatting = Formatting.Indented,
    Error = (object? _, ErrorEventArgs args) => {
      Logger.Error(args.ErrorContext.Error, "Error while serializing accounts in accounts file.");
      args.ErrorContext.Handled = true;
    },
    Culture = CultureInfo.InvariantCulture,
    MaxDepth = 10,
    DefaultValueHandling = DefaultValueHandling.Populate,
    ConstructorHandling = ConstructorHandling.Default,
  };

  /// <summary>
  /// Shows the error using the specified message
  /// </summary>
  /// <param name="message">The message</param>
  /// <param name="caption">The caption</param>
  /// <param name="callerName">The caller name</param>
  /// <param name="callerLineNumber">The caller line number</param>
  internal static void ShowError(string message, string caption, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0) {
    _ = CustomMessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0}\n\n{1} L{2}", message, callerName, callerLineNumber), caption, MessageBoxButton.OK, MessageBoxImage.Error);
  }

  /// <summary>
  /// Gets the git hash value from the assembly or null if it cannot be found.
  /// </summary>
  internal static string GetGitHash() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key.Equals("GitHash", StringComparison.Ordinal))?.Value ?? string.Empty;
  }

  /// <summary>
  /// Gets the build origin from the assembly or null if it cannot be found.
  /// </summary>
  internal static string GetBuildOrigin() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key.Equals("BuildOrigin", StringComparison.Ordinal))?.Value ?? string.Empty;
  }

  /// <summary>
  /// Gets the <see cref="Assembly" />'s version.
  /// </summary>
  /// <returns>The <see cref="Assembly" /> version as a string.</returns>
  internal static string GetAssemblyVersion() {
    var assembly = Assembly.GetExecutingAssembly();
    var auth = FileVersionInfo.GetVersionInfo(assembly.Location);
    return auth.FileVersion.ToStringSafe();
  }

  /// <summary>
  /// Gets the Unix timestamp in milliseconds from the current Date and Time.
  /// </summary>
  /// <returns>The Unix timestamp</returns>
  internal static long GetUnixMillis()
    => (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;

  /// <summary>
  /// Gets the application config path.
  /// </summary>
  /// <returns>The path to the application config.</returns>
  internal static string GetConfigPath()
    => Path.Combine(Paths.RoamingPath, "authConfigV1.json");

  /// <summary>
  /// Opens the website using the specified <paramref name="uri"/>.
  /// </summary>
  /// <param name="uri">The <see cref="Uri" /> of the website.</param>
  internal static void OpenWebsite(Uri uri) {
    Util.OpenWebsite(uri.ToString());
  }

  /// <summary>
  /// Opens the website using the specified <paramref name="url"/>
  /// </summary>
  /// <param name="url">The url of the website.</param>
  internal static void OpenWebsite(string url) {
    try {
      Util.StartProcessDetached(url);
#if DEBUG
    } catch (Exception exception) {
      Logger.Verbose(exception, "Failed to open url {0}.", url);
#else
    } catch {
      // Ignore...
#endif
    }
  }

  internal static void StartProcessDetached(string fileName, string? arguments = null) {
    var startInfo = new ProcessStartInfo {
      FileName = fileName,
      UseShellExecute = true,
    };
    if (arguments is not null) {
      startInfo.Arguments = arguments;
    }
    using var _ = Process.Start(startInfo);
  }

  internal static string GetEmailUri()
    => new StringBuilder().Append("mailto:").Append(Env.EmailToEmail.EncodeUriComponent())
      .Append("?subject=").Append(Env.EmailToSubject.EncodeUriComponent())
      .Append("&body=").Append(Env.EmailToBody.EncodeUriComponent())
      .ToString();
}
