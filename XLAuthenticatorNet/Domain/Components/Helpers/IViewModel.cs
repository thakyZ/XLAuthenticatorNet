using System;
using System.ComponentModel;
using System.Windows;
using XLAuthenticatorNet.Models.Abstracts;

namespace XLAuthenticatorNet.Domain.Components.Helpers;

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