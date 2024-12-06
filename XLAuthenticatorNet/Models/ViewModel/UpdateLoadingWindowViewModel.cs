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
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
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

  /// <summary>
  /// Refreshes the data using the specified update auto login disclaimer visibility
  /// </summary>
  /// <param name="updateAutoLoginDisclaimerVisibility">The update auto login disclaimer visibility</param>
  internal void RefreshData(bool updateAutoLoginDisclaimerVisibility = false) {
    base.RefreshData();

    if (updateAutoLoginDisclaimerVisibility) {
      this.NotifyPropertyChanged(nameof(this.AutoLoginDisclaimerVisibility));
    }
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public UpdateLoadingWindowViewModel() {}
#endif
}
