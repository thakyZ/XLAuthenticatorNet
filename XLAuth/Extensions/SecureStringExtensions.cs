using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security;

namespace XLAuth.Extensions;

/// <summary>
/// The secure string extensions class
/// </summary>
internal static class SecureStringExtensions {
  /// <summary>
  /// Returns the plain text using the specified secure string
  /// </summary>
  /// <param name="secureString">The secure string</param>
  /// <returns>The string</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static string ToPlainText(this SecureString secureString) => new NetworkCredential("", secureString).Password;

  /// <summary>
  /// Ises the null or empty using the specified secure string
  /// </summary>
  /// <param name="secureString">The secure string</param>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmpty(this SecureString? secureString) => secureString is null || new NetworkCredential("", secureString).Password.IsNullOrEmpty();

  /// <summary>
  /// Ises the null or white space using the specified secure string
  /// </summary>
  /// <param name="secureString">The secure string</param>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrWhiteSpace(this SecureString? secureString) => secureString is null || new NetworkCredential("", secureString).Password.IsNullOrWhiteSpace();

  /// <summary>
  /// Ises the null or empty or white space using the specified secure string
  /// </summary>
  /// <param name="secureString">The secure string</param>
  /// <returns>The bool</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static bool IsNullOrEmptyOrWhiteSpace(this SecureString? secureString) => secureString.IsNullOrEmpty() || secureString.IsNullOrWhiteSpace();
}
