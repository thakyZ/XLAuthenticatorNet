using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using XLAuthenticatorNet.Domain.Components.Helpers;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Models.Abstracts;

/// <summary>
/// <see href="https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit/blob/master/MaterialDesign3.Demo.Wpf/Domain/ViewModelBase.cs"/>
/// </summary>
/// <typeparam name="TFrameworkElement">A framework element to derive from.</typeparam>
internal abstract class ViewModelBase<TFrameworkElement> : IViewModel<TFrameworkElement> where TFrameworkElement : FrameworkElement {
  /// <summary>
  /// The parent element.
  /// </summary>
  private readonly TFrameworkElement? _parent;

  /// <summary>
  /// Gets the value of the parent
  /// </summary>
  public TFrameworkElement Parent {
    get {
      if (this._parent is not null) {
        return this._parent;
      }

      Logger.Warning("Failed to fetch parent control, likely a default constructor was called.");
      return null!;
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ViewModelBase{TFrameworkElement}"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  protected ViewModelBase(TFrameworkElement parent) {
    this._parent = parent;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="ViewModelBase{TFrameworkElement}"/> class
  /// </summary>
  protected ViewModelBase() { }

  public event PropertyChangedEventHandler? PropertyChanged;

  /// <summary>
  /// The changed type enum
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedType.Global"), SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal enum ChangedType {
    /// <summary>
    /// The text changed type
    /// </summary>
    Text = 0,
    /// <summary>
    /// The color changed type
    /// </summary>
    Color = 1,
  }

  /// <summary>
  /// Sets property if it does not equal existing value. Notifies listeners if change occurs.
  /// </summary>
  /// <typeparam name="TProperty">Type of property.</typeparam>
  /// <param name="member">The property's backing field.</param>
  /// <param name="value">The new value.</param>
  /// <param name="propertyName">Name of the property used to notify listeners.  This
  /// value is optional and can be provided automatically when invoked from compilers
  /// that support <see cref="CallerMemberNameAttribute"/>.</param>
  [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
  protected virtual bool SetProperty<TProperty>(ref TProperty member, TProperty value, [CallerMemberName] string propertyName = "") {
    if (EqualityComparer<TProperty>.Default.Equals(member, value)) {
      return false;
    }

    member = value;
    this.NotifyPropertyChanged(propertyName);
    return true;
  }

  /// <summary>
  /// Notifies listeners that a property value has changed.
  /// </summary>
  /// <param name="propertyName">Name of the property, used to notify listeners.</param>
  protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
    switch (propertyName) {
      case nameof(MainControlViewModel.OTPTimeLeft):
      case nameof(MainControlViewModel.OTPValue):
        Logger.Verbose("Property Changed: \"{0}\"", propertyName);
        goto default;
      default:
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return;
    }
  }

  /// <summary>
  /// Allows ViewModels to be reloaded from outside.
  /// <remarks><para>NOTE: May be able to be used maliciously since this method uses reflection.</para></remarks>
  /// </summary>
  public virtual void RefreshData() {
    Type type = this.GetType();

    if (!type.IsSubclassOfRawGeneric(typeof(ViewModelBase<>))) {
      Logger.Verbose("type is not subclass of IViewModel<> = {0}, Base = {1}", type.FullName, type.BaseType?.FullName ?? "None");
      return;
    }

    MethodInfo? methodInfo = type.GetMethods().FirstOrDefault(x => x.Name.Equals("NotifyPropertyChanged", StringComparison.Ordinal));
    bool useBuiltin = methodInfo is null;

    foreach (PropertyInfo property in type.GetProperties()) {
      Logger.Verbose("RefreshData PropertyChanged = {0}", property.Name);
      if (useBuiltin) {
        this.NotifyPropertyChanged(property.Name);
      } else {
        methodInfo?.Invoke(this, [property.Name]);
      }
    }
  }

  /// <summary>
  /// Releases the unmanaged resources
  /// </summary>
  protected virtual void ReleaseUnmanagedResources() {}

  /// <summary>
  /// Releases the managed resources
  /// </summary>
  [SuppressMessage("ReSharper", "VirtualMemberNeverOverridden.Global")]
  protected virtual void ReleaseManagedResources() {}

  /// <summary>
  /// Disposes the disposing
  /// </summary>
  /// <param name="disposing">The disposing</param>
  private void Dispose(bool disposing) {
    this.ReleaseUnmanagedResources();
    if (disposing) {
      this.ReleaseManagedResources();
    }
  }

  /// <summary>
  /// Disposes this instance
  /// </summary>
  public void Dispose() {
    this.Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  ~ViewModelBase() {
    this.Dispose(disposing: false);
  }
}
