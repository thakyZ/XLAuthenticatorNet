using System.Runtime.InteropServices;
using System.Windows;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The dialog result
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct DialogResult<T> {
  /// <summary>
  /// Gets the value of the dialog result.
  /// </summary>
  internal T? Value { get; init; }
  /// <summary>
  /// Gets the <see cref="MessageBoxResult" /> of the dialog result.
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