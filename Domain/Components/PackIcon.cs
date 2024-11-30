using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace XLAuthenticatorNet.Resources;

/// <summary>
/// The pack icon kind enum
/// </summary>
internal enum PackIconKind {
  /// <summary>
  /// The discord pack icon kind
  /// </summary>
  Discord,
}

/// <summary>
/// The pack icon data factory class
/// </summary>
internal static class PackIconDataFactory {
  /// <summary>
  /// Creates
  /// </summary>
  /// <returns>A dictionary of pack icon kind and string</returns>
  internal static IDictionary<PackIconKind, string> Create() => new Dictionary<PackIconKind, string> {
    {
      PackIconKind.Discord,
      "M22,24L16.75,19L17.38,21H4.5A2.5,2.5 0 0,1 2,18.5V3.5A2.5,2.5 0 0,1 4.5,1H19.5A2.5,2.5 0 0,1 22,3.5V24M12,6.8C9.32,6.8 7.44,7.95 7.44,7.95C8.47,7.03 10.27,6.5 10.27,6.5L10.1,6.33C8.41,6.36 6.88,7.53 6.88,7.53C5.16,11.12 5.27,14.22 5.27,14.22C6.67,16.03 8.75,15.9 8.75,15.9L9.46,15C8.21,14.73 7.42,13.62 7.42,13.62C7.42,13.62 9.3,14.9 12,14.9C14.7,14.9 16.58,13.62 16.58,13.62C16.58,13.62 15.79,14.73 14.54,15L15.25,15.9C15.25,15.9 17.33,16.03 18.73,14.22C18.73,14.22 18.84,11.12 17.12,7.53C17.12,7.53 15.59,6.36 13.9,6.33L13.73,6.5C13.73,6.5 15.53,7.03 16.56,7.95C16.56,7.95 14.68,6.8 12,6.8M9.93,10.59C10.58,10.59 11.11,11.16 11.1,11.86C11.1,12.55 10.58,13.13 9.93,13.13C9.29,13.13 8.77,12.55 8.77,11.86C8.77,11.16 9.28,10.59 9.93,10.59M14.1,10.59C14.75,10.59 15.27,11.16 15.27,11.86C15.27,12.55 14.75,13.13 14.1,13.13C13.46,13.13 12.94,12.55 12.94,11.86C12.94,11.16 13.45,10.59 14.1,10.59Z"
    },
  };
}

/// <summary>
/// The pack icon class
/// </summary>
/// <seealso cref="Control"/>
internal class PackIcon : Control {
  /// <summary>
  /// The create
  /// </summary>
  private static readonly Lazy<IDictionary<PackIconKind, string>> _dataIndex =
    new Lazy<IDictionary<PackIconKind, string>>(PackIconDataFactory.Create);

  /// <summary>
  /// Initializes a new instance of the <see cref="PackIcon"/> class
  /// </summary>
  static PackIcon() {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
  }

  /// <summary>
  /// The kind property changed callback
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind),
    typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(default(PackIconKind), KindPropertyChangedCallback));

  /// <summary>
  /// Kinds the property changed callback using the specified dependency object
  /// </summary>
  /// <param name="dependencyObject">The dependency object</param>
  /// <param name="dependencyPropertyChangedEventArgs">The dependency property changed event args</param>
  private static void KindPropertyChangedCallback(DependencyObject dependencyObject,
    DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs) =>
    ((PackIcon)dependencyObject).UpdateData();

  /// <summary>
  /// Gets or sets the icon to display.
  /// </summary>
  internal PackIconKind Kind {
    get => (PackIconKind)GetValue(KindProperty);
    set => SetValue(KindProperty, value);
  }

  /// <summary>
  /// The property metadata
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  private static readonly DependencyPropertyKey DataPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(""));

  /// <summary>
  /// The dependency property
  /// </summary>
  [SuppressMessage("ReSharper", "StaticMemberInGenericType"), SuppressMessage("ReSharper", "InconsistentNaming")]
  internal static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

  /// <summary>
  /// Gets the icon path data for the current <see cref="Kind"/>.
  /// </summary>
  [TypeConverter(typeof(GeometryConverter))]
  internal string? Data {
    get => (string?)GetValue(DataProperty);
    private set => SetValue(DataPropertyKey, value);
  }

  /// <summary>
  /// Ons the apply template
  /// </summary>
  public override void OnApplyTemplate() {
    base.OnApplyTemplate();
    UpdateData();
  }

  /// <summary>
  /// Updates the data
  /// </summary>
  private void UpdateData() {
    _dataIndex.Value.TryGetValue(this.Kind, out string? data);
    this.Data = data;
  }
}

/// <summary>
/// The pack icon extension class
/// </summary>
/// <seealso cref="MarkupExtension"/>
[MarkupExtensionReturnType(typeof(PackIcon))]
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class PackIconExtension : MarkupExtension {
  /// <summary>
  /// Initializes a new instance of the <see cref="PackIconExtension"/> class
  /// </summary>
  internal PackIconExtension() {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PackIconExtension"/> class
  /// </summary>
  /// <param name="kind">The kind</param>
  internal PackIconExtension(PackIconKind kind) {
    this.Kind = kind;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="PackIconExtension"/> class
  /// </summary>
  /// <param name="kind">The kind</param>
  /// <param name="size">The size</param>
  internal PackIconExtension(PackIconKind kind, double size) {
    this.Kind = kind;
    this.Size = size;
  }

  /// <summary>
  /// Gets or sets the value of the kind
  /// </summary>
  [ConstructorArgument("kind")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"),
   SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  internal PackIconKind Kind { get; set; }

  /// <summary>
  /// Gets or sets the value of the size
  /// </summary>
  [ConstructorArgument("size")]
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"),
   SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
  internal double? Size { get; set; }

  /// <summary>
  /// Provides the value using the specified service provider
  /// </summary>
  /// <param name="serviceProvider">The service provider</param>
  /// <returns>The result</returns>
  public override object ProvideValue(IServiceProvider serviceProvider) {
    var result = new PackIcon { Kind = this.Kind };

    if (!this.Size.HasValue) return result;

    result.Height = this.Size.Value;
    result.Width = this.Size.Value;

    return result;
  }
}