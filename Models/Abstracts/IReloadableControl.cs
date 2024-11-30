using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace XLAuthenticatorNet.Models.Abstracts;

/// <summary>
/// Allows ViewModels to be reloaded from outside.
/// </summary>
/// <typeparam name="TSource"></typeparam>
internal interface IReloadableControl<TSource> where TSource : FrameworkElement {
  /// <summary>
  /// Refreshes the data
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal void RefreshData();
}