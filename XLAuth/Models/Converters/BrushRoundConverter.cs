using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace XLAuth.Models.Converters;

/// <summary>
/// The brush round converter class
/// </summary>
/// <seealso cref="IValueConverter"/>
internal sealed class BrushRoundConverter : IValueConverter {
  /// <summary>
  /// Gets the value of the high value
  /// </summary>
  private Brush HighValue { get; } = Brushes.White;

  /// <summary>
  /// Gets the value of the low value
  /// </summary>
  private Brush LowValue { get; } = Brushes.Black;

  /// <summary>
  /// Converts the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="targetType">The target type</param>
  /// <param name="parameter">The parameter</param>
  /// <param name="culture">The culture</param>
  /// <returns>The object</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    if (value is not SolidColorBrush solidColorBrush) {
      return null;
    }

    var color = solidColorBrush.Color;

    var brightness = (0.3 * color.R)
                   + (0.59 * color.G)
                   + (0.11 * color.B);

    return brightness < 123 ? this.LowValue : this.HighValue;
  }

  /// <summary>
  /// Converts the back using the specified value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="targetType">The target type</param>
  /// <param name="parameter">The parameter</param>
  /// <param name="culture">The culture</param>
  /// <returns>The object</returns>
  public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    => Binding.DoNothing;
}
