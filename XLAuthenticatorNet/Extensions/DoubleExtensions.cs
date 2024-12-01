using System;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// Extension methods for the type of <cee langword="double" />.
/// </summary>
internal static class DoubleExtensions {
  /// <summary>
  /// Gets the value of the epsilon
  /// </summary>
  private static double Epsilon => 0.00001d;

  /// <summary>
  /// Equates two double values to each other.
  /// <seealso href="https://www.jetbrains.com/help/resharper/CompareOfFloatsByEqualityOperator.html" />
  /// <seealso href="https://rules.sonarsource.com/csharp/RSPEC-1244/" />
  /// </summary>
  /// <param name="double">The primary double value.</param>
  /// <param name="other">The double value to compare to.</param>
  /// <returns><see langword="true" /> if they are equal, otherwise <see langword="false" />.</returns>
  internal static bool Equals(this double @double, double other)
    => Math.Abs(@double - other) < Epsilon;

  /// <summary>
  /// Subtracts the largest using the specified double
  /// </summary>
  /// <param name="double">The double</param>
  /// <param name="other">The other</param>
  /// <returns>The double</returns>
  internal static double SubtractLargest(this double @double, double other) {
    var smallest = Math.Min(@double, other);
    var largest  = Math.Max(@double, other);
    return largest - smallest;
  }

  /// <summary>
  /// Subtracts the smallest using the specified double
  /// </summary>
  /// <param name="double">The double</param>
  /// <param name="other">The other</param>
  /// <returns>The double</returns>
  internal static double SubtractSmallest(this double @double, double other) {
    var smallest = Math.Min(@double, other);
    var largest  = Math.Max(@double, other);
    return smallest - largest;
  }
}