namespace XLAuthenticatorNet.Domain.Components.Helpers;

/// <summary>
/// The slide navigation subject interface
/// </summary>
internal interface ISlideNavigationSubject {
  /// <summary>
  /// Gets or sets the value of the active slide index
  /// </summary>
  int ActiveSlideIndex { get; set; }
}