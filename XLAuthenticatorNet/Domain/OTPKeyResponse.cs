using System.Diagnostics.CodeAnalysis;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The otp key response enum
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal enum OTPKeyResponse {
  /// <summary>
  /// The success otp key response
  /// </summary>
  Success = 0,
  /// <summary>
  /// The failed otp key response
  /// </summary>
  Failed = 1,
  /// <summary>
  /// The current account null otp key response
  /// </summary>
  CurrentAccountNull = 2,
  /// <summary>
  /// The launcher ip address null otp key response
  /// </summary>
  LauncherIpAddressNull = 3,
  /// <summary>
  /// The otp value null otp key response
  /// </summary>
  OTPValueNull = 4,
}