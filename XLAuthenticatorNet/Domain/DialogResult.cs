using System.Windows;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The dialog result
/// </summary>
internal readonly struct DialogResult {
  /// <summary>
  /// Gets the value of the value of the dialog result
  /// </summary>
  internal object? Value { get; init; }
  /// <summary>
  /// Gets the value of the <see cref="MessageBoxResult" /> of the dialog result
  /// </summary>
  internal MessageBoxResult Result { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DialogResult"/> class
  /// </summary>
  /// <param name="result">The result</param>
  /// <param name="value">The value</param>
  internal DialogResult(MessageBoxResult result, object? value = null) {
    this.Result = result;
    this.Value = value;
  }
}
