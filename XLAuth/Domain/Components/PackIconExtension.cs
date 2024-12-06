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
[SuppressMessage("ReSharper", "UnusedType.Global"),
 SuppressMessage("ReSharper", "MemberCanBePrivate.Global"),
 SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
internal sealed class PackIconExtension : MarkupExtension {
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
  internal PackIconKind Kind { get; set; }

  /// <summary>
  /// Gets or sets the value of the size
  /// </summary>
#pragma warning disable MA0083 // TODO: Wait for the issue to be fixed: https://github.com/meziantou/Meziantou.Analyzer/issues/775
  [ConstructorArgument("size")]
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
