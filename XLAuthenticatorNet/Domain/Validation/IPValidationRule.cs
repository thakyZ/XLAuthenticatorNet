using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace XLAuthenticatorNet.Domain.Validation;

/// <summary>
/// The IP validation rule
/// </summary>
/// <seealso cref="ValidationRule"/>
internal sealed class IPValidationRule : ValidationRule {
  /// <summary>
  /// Validates the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="cultureInfo">The culture info</param>
  /// <returns>The invalid result</returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo) {
    var invalidResult = new ValidationResult(isValid: false, "Field is required.");

    if (value is not string @string || !IPAddress.TryParse(@string, out IPAddress? address)) {
      return invalidResult;
    }

    // Extra layer to validate IPv4.
    if (address.AddressFamily == AddressFamily.InterNetwork && IPValidationRule.IsValidIpV4(@string)) {
      return ValidationResult.ValidResult;
    }

    if (address.AddressFamily == AddressFamily.InterNetworkV6) {
      return ValidationResult.ValidResult;
    }

    return invalidResult;
  }

  /// <summary>
  /// Tests if the string is a valid IPv4 using the specified string.
  /// </summary>
  /// <param name="value">The string to test against</param>
  /// <returns><see langword="true" /> if is valid; otherwise <see langword="false" />.</returns>
  private static bool IsValidIpV4(string value) {
    string[] parts = value.Split('.');
    if (parts.Length != 4) {
      return false;
    }
    return parts.All((string part) => int.TryParse(part, CultureInfo.InvariantCulture, out int @int) && @int is >= 0 and <= 255);
  }

  /// <summary>
  /// Tests if the string is a valid IP using the specified string.
  /// </summary>
  /// <param name="value">The string to test against</param>
  /// <returns><see langword="true" /> if is valid; otherwise <see langword="false" />.</returns>
  internal static bool IsValid(string value)
    => new IPValidationRule().Validate(value, CultureInfo.CurrentCulture) == ValidationResult.ValidResult;
}
