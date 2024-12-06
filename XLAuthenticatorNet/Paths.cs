using System;
using System.IO;

namespace XLAuthenticatorNet;

/// <summary>
/// The paths class
/// </summary>
internal static class Paths {
  /// <summary>
  /// Initializes a new instance of the <see cref="Paths"/> class
  /// </summary>
  static Paths() {
    RoamingPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "XIVLauncher", "XLAuth");
  }

  /// <summary>
  /// Gets or sets the value of the roaming path
  /// </summary>
  public static string RoamingPath { get; private set; }

  /// <summary>
  /// Gets the value of the resources path
  /// </summary>
  public static string ResourcesPath => Path.Combine(AppContext.BaseDirectory, "Resources");

  /// <summary>
  /// Overrides the roaming path using the specified path
  /// </summary>
  /// <param name="path">The path</param>
  public static void OverrideRoamingPath(string path) {
    RoamingPath = Path.Combine(Environment.ExpandEnvironmentVariables(path), "XLAuth");
  }
}
