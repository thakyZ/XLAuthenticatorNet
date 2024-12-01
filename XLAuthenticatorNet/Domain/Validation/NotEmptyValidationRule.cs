using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;
using XLAuthenticatorNet.Extensions;

namespace XLAuthenticatorNet.Domain.Validation;

/// <summary>
/// The not empty validation rule class
/// </summary>
/// <seealso cref="ValidationRule"/>
internal sealed class NotEmptyValidationRule : ValidationRule {
  /// <summary>
  /// Validates the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="cultureInfo">The culture info</param>
  /// <returns>The validation result</returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo) =>
    (value as string).IsNullOrEmptyOrWhiteSpace()
      ? new ValidationResult(isValid: false, "Field is required.")
      : ValidationResult.ValidResult;
}