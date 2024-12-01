using System.Diagnostics.CodeAnalysis;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The OTP key response enum
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
internal enum OTPKeyResponse {
  /// <summary>
  /// The success OTP key response
  /// </summary>
  Success = 0,
  /// <summary>
  /// The failed OTP key response
  /// </summary>
  Failed = 1,
  /// <summary>
  /// The current account null OTP key response
  /// </summary>
  CurrentAccountNull = 2,
  /// <summary>
  /// The launcher IP address null OTP key response
  /// </summary>
  LauncherIpAddressNull = 3,
  /// <summary>
  /// The OTP value null OTP key response
  /// </summary>
  OTPValueNull = 4,
}