using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuth.Domain.Components.Helpers;

/// <summary>
/// The navigation commands class
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming"),
 SuppressMessage("Style", "IDE1006:Naming Styles")]
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
