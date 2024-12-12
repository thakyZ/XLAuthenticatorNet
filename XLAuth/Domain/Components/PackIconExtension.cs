using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Markup;

// ReSharper disable once CheckNamespace
namespace XLAuth.Resources;

/// <summary>
/// The pack icon extension class
/// </summary>
/// <seealso cref="MarkupExtension"/>
[MarkupExtensionReturnType(typeof(PackIcon))]
internal sealed class PackIconExtension : MarkupExtension {
  /// <summary>
  /// Initializes a new instance of the <see cref="PackIconExtension"/> class
  /// </summary>
  internal PackIconExtension() { }

  /// <summary>
  /// Initializes a new instance of the <see cref="PackIconExtension"/> class
  /// </summary>
  /// <param name="kind">The kind</param>
  internal PackIconExtension(PackIconKind kind) {
    this.Kind = kind;
  }

  /// <summary>
  /// Gets or sets the value of the kind
  /// </summary>
  [ConstructorArgument("kind")]
  internal PackIconKind Kind { get; set; }

  /// <summary>
  /// Gets or sets the value of the size
  /// </summary>
  internal double? Size { get; set; }

  /// <summary>
  /// Provides the value using the specified service provider
  /// </summary>
  /// <param name="serviceProvider">The service provider</param>
  /// <returns>The result</returns>
  public override object ProvideValue(IServiceProvider serviceProvider) {
    var result = new PackIcon { Kind = this.Kind };

    if (!this.Size.HasValue) {
      return result;
    }

    result.Height = this.Size.Value;
    result.Width = this.Size.Value;

    return result;
  }
}
