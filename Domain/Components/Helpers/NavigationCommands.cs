using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Components.Helpers;

/// <summary>
/// The navigation commands class
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
internal static class NavigationCommands {
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand ShowSettingsCommand = new RoutedCommand();
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand HideSettingsCommand = new RoutedCommand();
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand GoBackCommand       = new RoutedCommand();
}