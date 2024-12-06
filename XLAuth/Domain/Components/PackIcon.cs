using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

// ReSharper disable once CheckNamespace
namespace XLAuth.Resources;

/// <summary>
/// The pack icon class
/// </summary>
/// <seealso cref="Control"/>
internal sealed class PackIcon : Control {
  /// <summary>
  /// The create
  /// </summary>
  private static readonly Lazy<IDictionary<PackIconKind, string>> _dataIndex = new(PackIconDataFactory.Create);

  /// <summary>
  /// Initializes a new instance of the <see cref="PackIcon"/> class
  /// </summary>
  static PackIcon() {
    DefaultStyleKeyProperty.OverrideMetadata(typeof(PackIcon), new FrameworkPropertyMetadata(typeof(PackIcon)));
  }

  /// <summary>
  /// The kind property changed callback
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming"),
   SuppressMessage("Style", "IDE1006:Naming Styles")]
  internal static readonly DependencyProperty KindProperty = DependencyProperty.Register(nameof(Kind),
    typeof(PackIconKind), typeof(PackIcon), new PropertyMetadata(default(PackIconKind), PackIcon.KindPropertyChangedCallback));

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
    get => (PackIconKind)this.GetValue(KindProperty);
    set => this.SetValue(KindProperty, value);
  }

  /// <summary>
  /// The property metadata
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming"),
   SuppressMessage("Style", "IDE1006:Naming Styles")]
  private static readonly DependencyPropertyKey DataPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Data), typeof(string), typeof(PackIcon), new PropertyMetadata(""));

  /// <summary>
  /// The dependency property
  /// </summary>
  [SuppressMessage("ReSharper", "InconsistentNaming"),
   SuppressMessage("Style", "IDE1006:Naming Styles")]
  internal static readonly DependencyProperty DataProperty = DataPropertyKey.DependencyProperty;

  /// <summary>
  /// Gets the icon path data for the current <see cref="Kind"/>.
  /// </summary>
  [TypeConverter(typeof(GeometryConverter))]
  internal string? Data {
    get => (string?)this.GetValue(DataProperty);
    private set => this.SetValue(DataPropertyKey, value);
  }

  /// <summary>
  /// Ons the apply template
  /// </summary>
  public override void OnApplyTemplate() {
    base.OnApplyTemplate();
    this.UpdateData();
  }

  /// <summary>
  /// Updates the data
  /// </summary>
  private void UpdateData() {
    _dataIndex.Value.TryGetValue(this.Kind, out var data);
    this.Data = data;
  }
}
