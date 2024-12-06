using System.Security;
using System.Windows.Data;
using XLAuth.Extensions;

namespace XLAuth.Models.Converters;

/// <summary>
/// The secure string converter class
/// </summary>
/// <seealso cref="IValueConverter"/>
public class SecureStringConverter : IValueConverter {
  /// <summary>
  /// Converts the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="targetType">The target type</param>
  /// <param name="parameter">The parameter</param>
  /// <param name="culture">The culture</param>
  /// <returns>The object</returns>
  public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
    Logger.Debug("Converting SecureString {0} [{1}]", value, value?.GetType().FullName ?? "null");
    if (value is SecureString secureString) {
      return secureString.ToPlainText();
    }

    if (value is string stringValue) {
      return stringValue;
    }

    return null;
  }

  /// <summary>
  /// Converts the back using the specified value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="targetType">The target type</param>
  /// <param name="parameter">The parameter</param>
  /// <param name="culture">The culture</param>
  /// <returns>The object</returns>
  public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
    Logger.Debug("Converting String {0} [{1}]", value, value?.GetType().FullName ?? "null");
    if (value is SecureString secureString) {
      return secureString;
    }

    if (value is string stringValue) {
      return stringValue.ToSecureString();
    }

    return null;
  }
}
