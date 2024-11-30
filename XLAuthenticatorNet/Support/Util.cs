using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Windows;

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
    _ = CustomMessageBox.Show($"{message}\n\n{callerName} L{callerLineNumber}", caption, MessageBoxButton.OK,
      MessageBoxImage.Error);
  }

  /// <summary>
  /// Gets the git hash value from the assembly
  /// or null if it cannot be found.
  /// </summary>
  internal static string GetGitHash() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value ?? string.Empty;
  }

  /// <summary>
  /// Gets the build origin from the assembly
  /// or null if it cannot be found.
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static string GetBuildOrigin() {
    var asm = Assembly.GetExecutingAssembly();
    IEnumerable<AssemblyMetadataAttribute> attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
    return attrs.FirstOrDefault(a => a.Key == "BuildOrigin")?.Value ?? string.Empty;
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
  /// Gets the unix millis
  /// </summary>
  /// <returns>The long</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static long GetUnixMillis() => (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

  /// <summary>
  /// Opens the website using the specified uri
  /// </summary>
  /// <param name="uri">The uri</param>
  internal static void OpenWebsite(string uri) {
    OpenWebsite(new Uri(uri));
  }

  /// <summary>
  /// Opens the website using the specified uri
  /// </summary>
  /// <param name="uri">The uri</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal static void OpenWebsite(Uri uri) {
    try {
      Process.Start(uri.ToString());
    } catch {
      // Ignore...
    }
  }

  /// <summary>
  /// Gets the from resources using the specified resource name
  /// </summary>
  /// <param name="resourceName">The resource name</param>
  /// <returns>The string</returns>
  internal static string? GetFromResources(string resourceName) {
    var asm = Assembly.GetExecutingAssembly();
    using Stream? stream = asm.GetManifestResourceStream(resourceName);
    if (stream is null) {
      ShowError($"Failed to load resource \"{resourceName}\".", "Failed to load resource");
      return null;
    }
    using var reader = new StreamReader(stream);

    return reader.ReadToEnd();
  }

  /// <summary>
  /// Gets the blank bitmap
  /// </summary>
  /// <returns>The bitmap source</returns>
  internal static BitmapSource GetBlankBitmap() {
      var output = new Bitmap(1, 1, PixelFormat.Alpha);
      output.SetPixel(0, 0, System.Drawing.Color.FromArgb(0, 0, 0, 0));
      return Imaging.CreateBitmapSourceFromHBitmap(output.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
          BitmapSizeOptions.FromEmptyOptions());
  }
}