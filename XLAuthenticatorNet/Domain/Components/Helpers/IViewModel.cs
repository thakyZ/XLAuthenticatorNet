using System;
using System.ComponentModel;
using System.Windows;
using XLAuthenticatorNet.Models.Abstracts;

namespace XLAuthenticatorNet.Domain.Components.Helpers;

/// <summary>
/// The view model interface
/// </summary>
/// <seealso cref="IReloadableControl{TSource}"/>
/// <seealso cref="INotifyPropertyChanged"/>
/// <seealso cref="IDisposable"/>
internal interface IViewModel<TSource> : IReloadableControl<TSource>, INotifyPropertyChanged, IDisposable where TSource : FrameworkElement {
    /// <summary>
    /// Gets the value of the parent
    /// </summary>
    internal TSource Parent { get; }
}