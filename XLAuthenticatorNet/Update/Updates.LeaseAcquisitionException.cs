using System;
using System.Diagnostics.CodeAnalysis;

namespace XLAuthenticatorNet.Update;

internal sealed partial class Updates {
  /// <summary>
  /// The lease acquisition exception class
  /// </summary>
  /// <seealso cref="Exception"/>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class LeaseAcquisitionException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseAcquisitionException"/> class
    /// </summary>
    /// <param name="message">The message</param>
    internal LeaseAcquisitionException(string message) : base($"Couldn't acquire lease: {message}") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseAcquisitionException"/> class
    /// </summary>
    public LeaseAcquisitionException() : base("Couldn't acquire lease") { }

    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseAcquisitionException"/> class
    /// </summary>
    /// <param name="message">The message</param>
    /// <param name="innerException">The inner exception</param>
    public LeaseAcquisitionException(string? message, Exception? innerException) : base($"Couldn't acquire lease: {message}", innerException) { }
  }
}