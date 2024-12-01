using System;
using System.Diagnostics.CodeAnalysis;

namespace XLAuthenticatorNet.Update;

internal sealed partial class Updates {
  /// <summary>
  /// The lease class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global"),
   SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal sealed class Lease {
    /// <summary>
    /// Gets or sets the value indicated if the lease was successful.
    /// </summary>
    internal bool Success { get; set; }

    /// <summary>
    /// Gets or sets the lease message.
    /// </summary>
    internal string? Message { get; set; }

    /// <summary>
    /// Gets or sets the value of the cut-off boot version.
    /// </summary>
    internal string? CutOffBootVer { get; set; }

    /// <summary>
    /// Gets or sets the value of the frontier url.
    /// </summary>
    internal string? FrontierUrl { get; set; }

    /// <summary>
    /// Gets or sets the value of the lease feature flags.
    /// </summary>
    internal LeaseFeatureFlags Flags { get; set; }

    /// <summary>
    /// Gets or sets the list of releases.
    /// </summary>
    internal string? ReleasesList { get; set; }

    /// <summary>
    /// Gets or sets the value of the valid until
    /// </summary>
    internal DateTime? ValidUntil { get; set; }
  }
}