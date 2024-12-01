using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Media.Animation;
using MaterialDesignThemes.Wpf.Transitions;

namespace XLAuthenticatorNet.Domain.Animations;

/// <summary>
/// The fade wipe class
/// </summary>
/// <seealso cref="ITransitionWipe"/>
internal sealed class FadeWipe : ITransitionWipe {
    /// <summary>
    /// The sine ease
    /// </summary>
    private readonly SineEase _sineEase = new();
    /// <summary>
    /// The zero
    /// </summary>
    private readonly KeyTime _zeroKeyTime = KeyTime.FromTimeSpan(TimeSpan.Zero);

    /// <summary>
    /// Duration of the animation
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"),
     SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
    internal TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(0.5);

    /// <summary>
    /// Wipes the from slide
    /// </summary>
    /// <param name="fromSlide">The from slide</param>
    /// <param name="toSlide">The to slide</param>
    /// <param name="_">The origin (unused)</param>
    /// <param name="zIndexController">The index controller</param>
    public void Wipe(TransitionerSlide fromSlide, TransitionerSlide toSlide, Point _, IZIndexController zIndexController) {
        // Set up time points
        var endKeyTime = KeyTime.FromTimeSpan(TimeSpan.FromSeconds(this.Duration.TotalSeconds / 2));

        // From
        var fromAnimation = new DoubleAnimationUsingKeyFrames();
        fromAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(1, this._zeroKeyTime));
        fromAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(0, endKeyTime, this._sineEase));

        // To
        var toAnimation = new DoubleAnimationUsingKeyFrames();
        toAnimation.KeyFrames.Add(new LinearDoubleKeyFrame(0, this._zeroKeyTime));
        toAnimation.KeyFrames.Add(new EasingDoubleKeyFrame(1, endKeyTime, this._sineEase));

        // Preset
        fromSlide.Opacity = 1;
        toSlide.Opacity = 0;

        // Set up events
        toAnimation.Completed += (object? _, EventArgs _) => {};
        fromAnimation.Completed += (object? _, EventArgs _) => {
            fromSlide.BeginAnimation(UIElement.OpacityProperty, animation: null);
            toSlide.BeginAnimation(UIElement.OpacityProperty, toAnimation);
        };

        // Animate
        fromSlide.BeginAnimation(UIElement.OpacityProperty, fromAnimation);
        zIndexController.Stack(toSlide, fromSlide);
    }
}
