using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Controls;

namespace XLAuthenticatorNet.Domain.Validation;

/// <summary>
/// The ip validation rule class
/// </summary>
/// <seealso cref="ValidationRule"/>
internal class IPValidationRule : ValidationRule {
  /// <summary>
  /// Validates the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="cultureInfo">The culture info</param>
  /// <returns>The invalid result</returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo) {
    var invalidResult = new ValidationResult(false, "Field is required.");

    if (value is not string @string || !IPAddress.TryParse(@string, out IPAddress? address)) {
      return invalidResult;
    }

    // Extra layer to validate IPv4.
    if (address.AddressFamily == AddressFamily.InterNetwork && IsValidIpV4(@string))
      return ValidationResult.ValidResult;
    if (address.AddressFamily == AddressFamily.InterNetworkV6)
      return ValidationResult.ValidResult;
    return invalidResult;
  }

  /// <summary>
  /// Ises the valid ip v 4 using the specified ip
  /// </summary>
  /// <param name="ip">The ip</param>
  /// <returns>The bool</returns>
  private static bool IsValidIpV4(string ip) {
    string[] parts = ip.Split('.');
    if (parts.Length != 4) {
      return false;
    }
    return parts.All((string part) => int.TryParse(part, out int @int) && @int is >= 0 and <= 255);
  }

  /// <summary>
  /// Ises the valid using the specified value
  /// </summary>
  /// <param name="value">The value</param>
  /// <returns>The bool</returns>
  internal static bool IsValid(string value) => new IPValidationRule().Validate(value, CultureInfo.CurrentCulture) == ValidationResult.ValidResult;
}