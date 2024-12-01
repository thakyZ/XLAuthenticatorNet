using System;

namespace XLAuthenticatorNet.Domain.Components.Helpers;

/// <summary>
/// The slide navigator frame class
/// </summary>
internal sealed class SlideNavigatorFrame {
  /// <summary>
  /// Gets the value of the slide index
  /// </summary>
  internal int SlideIndex { get; }

  /// <summary>
  /// Gets the value of the setup slide
  /// </summary>
  internal Action SetupSlide { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SlideNavigatorFrame"/> class
  /// </summary>
  /// <param name="slideIndex">The slide index</param>
  /// <param name="setupSlide">The setup slide</param>
  internal SlideNavigatorFrame(int slideIndex, Action setupSlide) {
    this.SlideIndex = slideIndex;
    this.SetupSlide = setupSlide;
  }
}