using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The string extensions class
/// </summary>
internal static class StringExtensions {
  /// <summary>
  /// Returns the string safe using the specified string
  /// </summary>
  /// <param name="string">The string</param>
  /// <param name="fallback">The fallback</param>
  /// <returns>The string</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string ToStringSafe(this string? @string, string fallback = "null")
    => @string ?? fallback;

  /// <summary>
  /// Returns the secure string using the specified string
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The secure string</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static SecureString? ToSecureString(this string? @string)
    => @string.IsNullOrEmptyOrWhiteSpace() ? null : new NetworkCredential("", @string).SecurePassword;

  /// <summary>
  /// Returns the byte array using the specified string
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The byte array</returns>
  internal static byte[] ToByteArray(this string @string) {
    try {
      return Enumerable.Range(0, @string.Length).Where(x => x % 2 == 0).Select(x => Convert.ToByte(@string.Substring(x, 2), 16)).ToArray();
    } catch {
      // Ignore...
      return Encoding.Default.GetBytes(@string);
    }
  }

  /// <summary>
  /// Splits the line endings using the specified string
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The string array</returns>
  internal static string[] SplitLineEndings(this string @string)
      => @string.ReplaceLineEndings().Split(Environment.NewLine);

  /// <summary>
  /// Compacts the multiline string using the specified string
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The string</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string CompactMultilineString(this string @string)
    => @string.ReplaceLineEndings(" ").TrimStart(' ').TrimEnd(' ');

  /// <summary>
  /// Tests if the string is null or empty
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmpty([NotNullWhen(false)] this string? @string)
    => string.IsNullOrEmpty(@string);

  /// <summary>
  /// Tests if the string is the null or white space
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? @string)
    => string.IsNullOrWhiteSpace(@string);

  /// <summary>
  /// Tests if the specified string is the null or empty or white space
  /// </summary>
  /// <param name="string">The string</param>
  /// <returns>The bool</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmptyOrWhiteSpace([NotNullWhen(false)] this string? @string)
    => @string.IsNullOrEmpty() || @string.IsNullOrWhiteSpace();

  /// <summary>
  /// Tests if the string is a number constrained to the number of digits
  /// </summary>
  /// <param name="string">The string</param>
  /// <param name="digits">The digits</param>
  /// <returns>The bool</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNumberOf([NotNullWhen(true)] this string? @string, int digits = 1)
    => @string?.ToCharArray() is char[] chars && chars.All(char.IsDigit) && chars.Length == digits;
}