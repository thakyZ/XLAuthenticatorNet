using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Windows;
using Env = DotEnv.Generated.Environment;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The util class
/// </summary>
internal static class Util {
  /// <summary>
  /// Shows the error using the specified message
  /// </summary>
  /// <param name="message">The message</param>
  /// <param name="caption">The caption</param>
  /// <param name="callerName">The caller name</param>
  /// <param name="callerLineNumber">The caller line number</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal static void ShowError(string message, string caption, [CallerMemberName] string callerName = "", [CallerLineNumber] int callerLineNumber = 0) {
    _ = CustomMessageBox.Show(string.Format(CultureInfo.InvariantCulture, "{0}\n\n{1} L{2}", message, callerName, callerLineNumber), caption, MessageBoxButton.OK, MessageBoxImage.Error);
  }

  /// <summary>
  /// Gets the git hash value from the assembly
  /// or null if it cannot be found.
  /// </summary>
  internal static string GetGitHash() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key.Equals("GitHash", StringComparison.Ordinal))?.Value ?? string.Empty;
  }

  /// <summary>
  /// Gets the build origin from the assembly
  /// or null if it cannot be found.
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static string GetBuildOrigin() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key.Equals("BuildOrigin", StringComparison.Ordinal))?.Value ?? string.Empty;
  }

  /// <summary>
  /// Gets the assembly version
  /// </summary>
  /// <returns>The string</returns>
  internal static string GetAssemblyVersion() {
    var assembly = Assembly.GetExecutingAssembly();
    var auth = FileVersionInfo.GetVersionInfo(assembly.Location);
    return auth.FileVersion.ToStringSafe();
  }

  /// <summary>
  /// Gets the Unix timestamp in milliseconds from the date and time of now.
  /// </summary>
  /// <returns>The Unix timestamp</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static long GetUnixMillis()
    => (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalMilliseconds;

  /// <summary>
  /// Opens the website using the specified url
  /// </summary>
  /// <param name="url">The url of the website</param>
  internal static void OpenWebsite(string url) {
    var uri = new Uri(url);
    Util.OpenWebsite(uri);
  }

  /// <summary>
  /// Opens the website using the specified <paramref name="uri"/>
  /// </summary>
  /// <param name="uri">The url of the website</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal static void OpenWebsite(Uri uri) {
    try {
      var url = uri.ToString();
      Process.Start(url);
#if DEBUG
    } catch (Exception exception) {
      Logger.Verbose(exception, "Failed to open uri {0}.", uri);
    }
#else
    } catch {
      // Ignore...
    }
#endif
  }

  internal static string GetEmailUri()
    => new StringBuilder().Append("mailto:").Append(Env.EmailToEmail.EncodeUriComponent())
      .Append("?subject=").Append(Env.EmailToSubject.EncodeUriComponent())
      .Append("&body=").Append(Env.EmailToBody.EncodeUriComponent())
      .ToString();
}
