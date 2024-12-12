using System.Net;
using System.Runtime.CompilerServices;
using System.Security;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="SecureString" /> type.
/// </summary>
internal static class SecureStringExtensions {
  /// <summary>
  /// Returns the plain text <see langword="string" /> using the specified <see cref="SecureString" />.
  /// </summary>
  /// <param name="secureString">The <see cref="SecureString" /> to query.</param>
  /// <returns>The raw secured <see langword="string" />.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string ToPlainText(this SecureString secureString)
    => new NetworkCredential("", secureString).Password;

  /*
  /// <summary>
  /// Indicates whether the specified <see cref="SecureString" />'s value is <see langword="null"/> or an empty string ("").
  /// </summary>
  /// <param name="secureString">The <see cref="SecureString" /> to test.</param>
  /// <returns><see langword="true"/> if the <paramref name="value" /> parameter is <see langword="null"/> or an empty string (""); otherwise, <see langword="false"/>.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmpty([NotNullWhen(false)] this SecureString? secureString)
    => secureString?.ToPlainText().IsNullOrEmpty() != false;
  */

  /*
  /// <summary>
  /// Indicates whether a specified <see cref="SecureString" />'s value is <see langword="null"/>, empty, or consists only of white-space characters.
  /// </summary>
  /// <param name="secureString">The <see cref="SecureString" /> to test.</param>
  /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrWhiteSpace([NotNullWhen(false)] this SecureString? secureString)
    => secureString?.ToPlainText().IsNullOrWhiteSpace() != false;
  */

  /// <summary>
  /// Indicates whether a specified <see cref="SecureString" />'s value is <see langword="null"/>, empty, or consists only of white-space characters.
  /// </summary>
  /// <param name="secureString">The <see cref="SecureString" /> to test.</param>
  /// <returns><see langword="true"/> if the value parameter is <see langword="null"/> or <see cref="string.Empty"/>, or if <paramref name="value" /> consists exclusively of white-space characters.</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmptyOrWhiteSpace([NotNullWhen(false)] this SecureString? secureString)
    => secureString?.ToPlainText().IsNullOrEmptyOrWhiteSpace() != false;
}
