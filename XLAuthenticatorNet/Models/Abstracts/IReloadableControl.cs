namespace XLAuthenticatorNet.Models.Abstracts;

/// <summary>
/// Allows ViewModels to be reloaded from outside.
/// </summary>
internal interface IReloadableControl {
  /// <summary>
  /// Refreshes the data
  /// </summary>
  internal void RefreshData();
}
