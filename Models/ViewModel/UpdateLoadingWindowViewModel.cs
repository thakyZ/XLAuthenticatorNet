using System.Diagnostics.CodeAnalysis;
using System.Windows;
using CheapLoc;
using XLAuthenticatorNet.Models.Abstracts;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The update loading window view model class
/// </summary>
/// <seealso cref="ViewModelBase{UpdateLoadingWindow}"/>
internal class UpdateLoadingWindowViewModel : ViewModelBase<UpdateLoadingWindow> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  public string Title => Loc.Localize(nameof(UpdateLoadingWindow) + nameof(Title), "Loading Update...");
  /// <summary>
  /// Gets the value of the update check label
  /// </summary>
  public string UpdateCheckLabel => Loc.Localize(nameof(UpdateCheckLabel), "Checking for updates...");
  /// <summary>
  /// Gets the value of the auto login hint label
  /// </summary>
  public string AutoLoginHintLabel => Loc.Localize(nameof(AutoLoginHintLabel), "Hold the shift key to change settings!");
  /// <summary>
  /// Gets the value of the reset uid cache hint label
  /// </summary>
  public string ResetUidCacheHintLabel => Loc.Localize(nameof(ResetUidCacheHintLabel), "Hold the control key to reset UID cache!");

  /// <summary>
  /// Gets the value of the auto login disclaimer visibility
  /// </summary>
  public Visibility AutoLoginDisclaimerVisibility => App.Settings.CloseApp ? Visibility.Visible : Visibility.Collapsed;
  /// <summary>
  /// Gets the value of the reset uid cache disclaimer visibility
  /// </summary>
  public Visibility ResetUidCacheDisclaimerVisibility => App.Settings.UniqueIdCacheEnabled ? Visibility.Visible : Visibility.Collapsed;
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
    private set => SetProperty(ref this._updateLoadingCardHeight, value);
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
    if (AutoLoginDisclaimerVisibility == Visibility.Visible && ResetUidCacheDisclaimerVisibility == Visibility.Visible) {
      UpdateLoadingCardHeight = _UPDATE_LOADING_CARD_HEIGHT_DEFAULT + 19;
    }
    return output;
  }

  /// <summary>
  /// Refreshes the data using the specified update auto login disclaimer visibility
  /// </summary>
  /// <param name="updateAutoLoginDisclaimerVisibility">The update auto login disclaimer visibility</param>
  /// <param name="updateResetUidCacheDisclaimerVisibility">The update reset uid cache disclaimer visibility</param>
  internal void RefreshData(bool updateAutoLoginDisclaimerVisibility = false, bool updateResetUidCacheDisclaimerVisibility = false) {
    base.RefreshData();

    if (updateAutoLoginDisclaimerVisibility) {
      NotifyPropertyChanged(nameof(AutoLoginDisclaimerVisibility));
    }
    if (updateResetUidCacheDisclaimerVisibility) {
      NotifyPropertyChanged(nameof(ResetUidCacheDisclaimerVisibility));
    }
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public UpdateLoadingWindowViewModel() {}
#endif
}