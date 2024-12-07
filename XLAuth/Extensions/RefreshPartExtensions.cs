namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="RefreshPart" /> type.
/// </summary>
internal static class RefreshPartExtensions {
  /// <summary>
  /// Checks if the provided <see cref="RefreshPart" /> contains a flag value.
  /// </summary>
  /// <param name="value">The <see cref="RefreshPart" /> to check.</param>
  /// <param name="flags">The <see cref="RefreshPart" /> value to check against.</param>
  /// <returns><see langword="true" /> if the <paramref name="value"/> contains the <paramref name="flags"/> or contains <see cref="RefreshPart.UpdateAll" />; otherwise <see langword="false" />.</returns>
  public static bool Contains(this RefreshPart value, RefreshPart flags) {
    return (value & flags) != RefreshPart.None || value.Contains(RefreshPart.UpdateAll);
  }
}
