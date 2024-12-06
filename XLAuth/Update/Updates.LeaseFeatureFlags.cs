using System;
using System.Diagnostics.CodeAnalysis;

namespace XLAuth.Update;

internal sealed partial class Updates {
  /// <summary>
  /// The lease feature flags enum
  /// </summary>
  [Flags]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal enum LeaseFeatureFlags {
    /// <summary>
    /// Specifies no feature flags
    /// </summary>
    None = 0,

    /// <summary>
    /// Specifies to disable updates for <see cref="XLAuth" />.
    /// </summary>
    GlobalDisableXLAuth = 1,

    /// <summary>
    /// Specifies to force updates for <see cref="XLAuth" /> through a proxy.
    /// </summary>
    ForceProxyXLAuthAndAssets = 1 << 1,
  }
}
