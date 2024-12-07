using System.Net;
using System.Runtime.CompilerServices;
using System.Security;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see langword="string" /> type.
/// </summary>
internal static class StringExtensions {
  /// <summary>
  /// Returns the <see langword="string"/> unless it is <see langword="null"/> where it will use the fallback instead.
  /// </summary>
  /// <param name="string">The <see langword="string"/> to return.</param>
  /// <param name="fallback">The fallback <see langword="string"/> if <paramref name="string"/> is <see langword="null"/>.</param>
  /// <returns>The <see langword="string"/> or the fallback <see langword="string"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string ToStringSafe(this string? @string, string fallback = "null")
    => @string ?? fallback;

  /// <summary>
  /// Returns a <see cref="SecureString"/> using the specified <see langword="string"/>.
  /// </summary>
  /// <param name="string">The <see langword="string"/> to convert.</param>
  /// <returns>An instance of the <see cref="SecureString"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static SecureString? ToSecureString(this string? @string)
    => @string.IsNullOrEmptyOrWhiteSpace() ? null : new NetworkCredential("", @string).SecurePassword;

  /// <summary>
  /// Returns a <see langword="byte"/> <see cref="Array"/> using the specified <see langword="string"/>.
  /// </summary>
  /// <param name="string">The <see langword="string"/> to convert.</param>
  /// <returns>An <see cref="Array"/> of <see langword="byte"/>s from the specified <see langword="string"/>.</returns>
  internal static byte[] ToByteArray(this string @string) {
    try {
      return Enumerable.Range(0, @string.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(@string.Substring(x, 2), 16)).ToArray();
    } catch {
      // Ignore...
      return Encoding.Default.GetBytes(@string);
    }
  }

  /// <summary>
  /// Splits the line endings using the specified <see langword="string"/>.
  /// </summary>
  /// <param name="string">The <see langword="string"/> to split.</param>
  /// <returns>An <see cref="Array"/> containing each line in the <see langword="string"/>.</returns>
  internal static string[] SplitLineEndings(this string @string)
      => @string.ReplaceLineEndings().Split(Environment.NewLine);

  /// <summary>
  /// Compacts the raw <see langword="string"/> using the specified <see langword="string"/>
  /// </summary>
  /// <param name="string">The raw <see langword="string"/> to compact.</param>
  /// <returns>The string with all trailing white spaces and line endings trimmed.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string CompactRawString(this string @string)
    => @string.ReplaceLineEndings(" ").TrimStart(' ').TrimEnd(' ');

  /// <summary>
  /// Indicates whether the specified string is <see langword="null"/> or an empty string ("").
  /// </summary>
  /// <param name="value">The <see langword="string"/> to test.</param>
  /// <returns><see langword="true"/> if the <paramref name="value" /> parameter is <see langword="null"/> or an empty string (""); otherwise, <see langword="false"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmpty([NotNullWhen(false)] this string? value)
    => string.IsNullOrEmpty(value);

  /// <summary>
  /// Indicates whether a specified string is <see langword="null"/>, empty, or consists only of white-space characters.
  /// </summary>
  /// <param name="value">The <see langword="string"/> to test.</param>
  /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? value)
    => string.IsNullOrWhiteSpace(value);

  /// <summary>
  /// Indicates whether a specified string is <see langword="null"/>, empty, or consists only of white-space characters.
  /// </summary>
  /// <param name="value">The <see langword="string"/> to test.</param>
  /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmptyOrWhiteSpace([NotNullWhen(false)] this string? value)
    => value.IsNullOrEmpty() || value.IsNullOrWhiteSpace();

  /// <summary>
  /// Tests if the <see langword="string"/> is a number constrained to the number of digits
  /// </summary>
  /// <param name="string">The <see langword="string"/> to test.</param>
  /// <param name="digits">The number of digits to test for.</param>
  /// <returns><see langword="true"/> if the <see langword="string"/> is a match; otherwise <see langword="false"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNumberOf([NotNullWhen(true)] this string? @string, int digits = 1)
    => @string?.ToCharArray() is char[] chars && chars.All(char.IsDigit) && chars.Length == digits;

  /// <summary>
  /// Encodes special characters in a <see langword="string"/> to be used in a <see cref="Uri" /> query.
  /// </summary>
  /// <param name="string">The <see langword="string"/> to encode.</param>
  /// <returns>The encoded <see langword="string"/>.</returns>
  internal static string EncodeUriComponent(this string @string)
    => Uri.EscapeDataString(@string);
}
