using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuth.Domain.Components.Helpers;

/// <summary>
/// The navigation commands class
/// </summary>
internal static class NavigationCommands {
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand ShowSettingsCommand = new();
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand HideSettingsCommand = new();
  /// <summary>
  /// The routed command
  /// </summary>
  internal static readonly RoutedCommand GoBackCommand       = new();
}
