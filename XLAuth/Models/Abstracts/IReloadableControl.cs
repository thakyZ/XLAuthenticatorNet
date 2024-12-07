namespace XLAuth.Models.Abstracts;

/// <summary>
/// Allows ViewModels to be reloaded from outside.
/// </summary>
internal interface IReloadableControl {
  /// <summary>
  /// Allows ViewModels to be reloaded from outside.
  /// <remarks><para>NOTE: May be able to be used maliciously since this method uses reflection.</para></remarks>
  /// </summary>
  /// <param name="part">Flags to represent which parts of the model/view to refresh.</param>
  internal void RefreshData(RefreshPart part);
}
