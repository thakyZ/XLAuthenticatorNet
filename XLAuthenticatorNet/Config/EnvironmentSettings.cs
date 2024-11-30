using System;

namespace XLAuthenticatorNet.Config {

  /// <summary>
  /// The environment settings class
  /// </summary>
  internal static class EnvironmentSettings {
    /// <summary>
    /// Gets the value of the is wine
    /// </summary>
    internal static bool IsWine => CheckEnvBool("XL_WINEONLINUX");
    /// <summary>
    /// Gets the value of the is hardware rendered
    /// </summary>
    internal static bool IsHardwareRendered => CheckEnvBool("XL_HWRENDER");
    /// <summary>
    /// Gets the value of the is disable updates
    /// </summary>
    internal static bool IsDisableUpdates => CheckEnvBool("XL_NOAUTOUPDATE");
    /// <summary>
    /// Gets the value of the is pre release
    /// </summary>
    internal static bool IsPreRelease => CheckEnvBool("XL_RERELEASE");
    /// <summary>
    /// Gets the value of the is no kill switch
    /// </summary>
    internal static bool IsNoKillSwitch => CheckEnvBool("XL_NO_KILLSWITCH");
    /// <summary>
    /// Gets the value of the is no runas
    /// </summary>
    internal static bool IsNoRunas => CheckEnvBool("XL_AUTH_NO_RUNAS");
    /// <summary>
    /// Gets the value of the is ignore space requirements
    /// </summary>
    internal static bool IsIgnoreSpaceRequirements => CheckEnvBool("XL_NO_SPACE_REQUIREMENTS");
    /// <summary>
    /// Gets the value of the is open steam minimal
    /// </summary>
    internal static bool IsOpenSteamMinimal => CheckEnvBool("XL_OPEN_STEAM_MINIMAL");
    /// <summary>
    /// Checks the env bool using the specified var
    /// </summary>
    /// <param name="var">The var</param>
    /// <returns>The bool</returns>
    private static bool CheckEnvBool(string var) => bool.Parse(Environment.GetEnvironmentVariable(var) ?? "false");
  }
}