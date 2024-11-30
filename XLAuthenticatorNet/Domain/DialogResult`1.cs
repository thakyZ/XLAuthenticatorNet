using System.Windows;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The dialog result
/// </summary>
public struct DialogResult<T> {
  /// <summary>
  /// Gets or inits the value of the value
  /// </summary>
  internal T? Value { get; init; }
  /// <summary>
  /// Gets or inits the value of the result
  /// </summary>
  internal MessageBoxResult Result { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="DialogResult"/> class
  /// </summary>
  /// <param name="result">The result</param>
  /// <param name="value">The value</param>
  internal DialogResult(MessageBoxResult result, T? value = default) {
    this.Result = result;
    this.Value = value;
  }
}