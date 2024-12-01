using System;
using System.Globalization;
using System.Runtime.InteropServices;
using XLAuthenticatorNet.Extensions;

namespace XLAuthenticatorNet.Models.Exceptions;

/// <summary>
/// The exception that is thrown when the program is running on an Operating system that is not supported.
/// </summary>
[Serializable]
internal sealed class UnsupportedOSException : NotSupportedException {
  public UnsupportedOSException() : base(UnsupportedOSException.GenerateMessage()) {}

  public UnsupportedOSException(string? message) : base(UnsupportedOSException.GenerateMessage(message)) {}

  public UnsupportedOSException(string? message, Exception? innerException) : base(UnsupportedOSException.GenerateMessage(message), innerException) {}

  private static string GenerateMessage(string? message = null)
    => string.Format(CultureInfo.InvariantCulture, "The current host operating system, {0}, is not supported.", RuntimeInformation.OSDescription)
      + (message.IsNullOrEmptyOrWhiteSpace() ? string.Empty : string.Format(CultureInfo.InvariantCulture, "{0}{1}", Environment.NewLine, message));
}
