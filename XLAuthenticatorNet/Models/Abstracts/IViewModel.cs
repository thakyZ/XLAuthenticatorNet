using System;
using System.ComponentModel;
using System.Windows;

namespace XLAuthenticatorNet.Models.Abstracts;

/// <summary>
/// The view model interface
/// </summary>
/// <seealso cref="IReloadableControl"/>
/// <seealso cref="INotifyPropertyChanged"/>
/// <seealso cref="IDisposable"/>
internal interface IViewModel<out TFrameworkElement> : IReloadableControl, INotifyPropertyChanged, IDisposable where TFrameworkElement : FrameworkElement {
  /// <summary>
  /// Gets the value of the parent
  /// </summary>
  internal TFrameworkElement Parent { get; }
}
