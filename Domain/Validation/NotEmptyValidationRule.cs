using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Windows.Controls;
using XLAuthenticatorNet.Extensions;

namespace XLAuthenticatorNet.Domain.Validation;

/// <summary>
/// The not empty validation rule class
/// </summary>
/// <seealso cref="ValidationRule"/>
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class NotEmptyValidationRule : ValidationRule {
  /// <summary>
  /// Validates the value
  /// </summary>
  /// <param name="value">The value</param>
  /// <param name="cultureInfo">The culture info</param>
  /// <returns>The validation result</returns>
  public override ValidationResult Validate(object? value, CultureInfo cultureInfo) =>
    (value as string).IsNullOrEmptyOrWhiteSpace()
      ? new ValidationResult(false, "Field is required.")
      : ValidationResult.ValidResult;
}