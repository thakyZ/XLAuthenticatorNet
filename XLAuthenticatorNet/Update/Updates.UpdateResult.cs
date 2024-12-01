using System;

using Microsoft.Extensions.Logging;

using Velopack;
using Velopack.Locators;
using Velopack.Sources;
using Velopack.Windows;

using XLAuthenticatorNet.Models.Exceptions;
using XLAuthenticatorNet.Support;

namespace XLAuthenticatorNet.Update;

internal sealed partial class Updates {
  /// <summary>
  /// The update result class
  /// </summary>
  private sealed class UpdateResult {
    /// <summary>
    /// Gets or sets the value of the manager
    /// </summary>
    internal UpdateManager Manager { get; }

    /// <summary>
    /// Gets or sets the value of the lease
    /// </summary>
    internal Lease Lease { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateResult"/> class
    /// </summary>
    /// <param name="manager">The instance of <see cref="UpdateManager" />.</param>
    /// <param name="lease">The instance of <see cref="Lease" />.</param>
    internal UpdateResult(UpdateManager manager, Lease lease) {
      this.Lease   = lease;
      this.Manager = manager;
    }
  }
}