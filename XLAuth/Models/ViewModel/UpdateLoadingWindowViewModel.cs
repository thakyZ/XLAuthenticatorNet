using System.Diagnostics.CodeAnalysis;
using System.Windows;
using CheapLoc;

using XLAuth.Extensions;
using XLAuth.Models.Abstracts;
using XLAuth.Windows;

namespace XLAuth.Models.ViewModel;

/// <summary>
/// The update loading window view model class
/// </summary>
/// <seealso cref="ViewModelBase{UpdateLoadingWindow}"/>
internal sealed class UpdateLoadingWindowViewModel : ViewModelBase<UpdateLoadingWindow> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title
    => Loc.Localize(nameof(UpdateLoadingWindow) + nameof(this.Title), "Loading Update...");

  /// <summary>
  /// Gets the value of the update check label
  /// </summary>
  public string UpdateCheckLabel
    => Loc.Localize(nameof(this.UpdateCheckLabel), "Checking for updates...");

  /// <summary>
  /// Gets the value of the auto login hint label
  /// </summary>
  public string AutoLoginHintLabel
    => Loc.Localize(nameof(this.AutoLoginHintLabel), "Hold the shift key to change settings!");

  /// <summary>
  /// Gets the value of the auto login disclaimer visibility
  /// </summary>
  public Visibility AutoLoginDisclaimerVisibility
    => App.Settings.CloseApp ? Visibility.Visible : Visibility.Collapsed;

  /// <summary>
  /// The update loading card height default
  /// </summary>
  private const double _UPDATE_LOADING_CARD_HEIGHT_DEFAULT = 149.0d;

  /// <summary>
  /// The update loading card height
  /// </summary>
  private double _updateLoadingCardHeight = 149.0d;

  /// <summary>
  /// Gets or sets the value of the update loading card height
  /// </summary>
  public double UpdateLoadingCardHeight {
    get => this._updateLoadingCardHeight;
    private set => this.SetProperty(ref this._updateLoadingCardHeight, value);
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="UpdateLoadingWindowViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal UpdateLoadingWindowViewModel(UpdateLoadingWindow parent) : base(parent) { }

  /// <inheritdoc />
  protected override bool SetProperty<TProperty>(ref TProperty member, TProperty value, string propertyName = "") {
    bool output = base.SetProperty(ref member, value, propertyName);
    // ReSharper disable once InvertIf
    if (this.AutoLoginDisclaimerVisibility == Visibility.Visible) {
      this.UpdateLoadingCardHeight = _UPDATE_LOADING_CARD_HEIGHT_DEFAULT + 19;
    }
    return output;
  }

  /// <inheritdoc cref="XLAuth.Models.Abstracts.IReloadableControl.RefreshData(RefreshPart)"/>
  public override void RefreshData(RefreshPart part) {
    base.RefreshData(part);

    if (part.Contains(RefreshPart.UpdateAutoLoginDisclaimerVisibility)) {
      this.NotifyPropertyChanged(nameof(this.AutoLoginDisclaimerVisibility));
    }
  }

#if DEBUG
  public UpdateLoadingWindowViewModel() {}
#endif
}
