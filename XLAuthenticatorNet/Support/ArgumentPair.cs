namespace XLAuthenticatorNet.Support;

/// <summary>
/// The argument pair class
/// </summary>
internal abstract class ArgumentPair {
  /// <summary>
  /// Gets or inits the value of the name
  /// </summary>
  internal string Name { get; init; }
  /// <summary>
  /// Gets or inits the value of the value
  /// </summary>
  internal object? Value { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ArgumentPair"/> class
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="value">The value</param>
  protected ArgumentPair(string name, object? value) {
    this.Name = name;
    this.Value = value;
  }
}