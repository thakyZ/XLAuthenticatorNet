using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// The main control class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class MainControl : UserControl {
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private MainControlViewModel ViewModel
    => (this.DataContext as MainControlViewModel)!;

  /// <summary>
  /// Gets the value of the parent window
  /// </summary>
  public required FrameworkElement ParentWindow { get; init; }

  /// <summary>
  /// The background
  /// </summary>
  private readonly Dictionary<int, (Brush Foreground, Brush Background)> _progressBarThemes = new(2);

  /// <summary>
  /// Initializes a new instance of the <see cref="MainControl"/> class
  /// </summary>
  internal MainControl() {
    this.InitializeComponent();
    this.DataContext = new MainControlViewModel(this);
  }

  /// <summary>
  /// Updates the progress bar theme
  /// </summary>
  private void UpdateProgressBarTheme() {
    try {
      Theme? theme = Util.GetTheme();
      if (theme is null || this._progressBarThemes.Count == 2) {
        return;
      }
      Logger.Debug("Update progress bar themes.");
      this._progressBarThemes[0] = (Foreground: new SolidColorBrush(theme.PrimaryDark.ForegroundColor ?? theme.PrimaryDark.Color),
                                    Background: new SolidColorBrush(theme.PrimaryLight.ForegroundColor ?? theme.PrimaryLight.Color));
      this._progressBarThemes[1] = (Foreground: new SolidColorBrush(MaterialDesignColors.Recommended.RedSwatch.Red700),
                                    Background: new SolidColorBrush(MaterialDesignColors.Recommended.RedSwatch.Red200));
    } catch (Exception e) {
      Logger.Error(e, "Failed to set progress bar theme.");
    }
  }

  /// <summary>
  /// Otps the time left on value changed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void OTPTimeLeft_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e) {
    if (sender is not ProgressBar progressbar || this._progressBarThemes.Keys.Count != 2) {
      return;
    }

    if (e.NewValue < 8) {
      progressbar.Background = this._progressBarThemes[1].Background;
      progressbar.BorderBrush = this._progressBarThemes[1].Background;
      progressbar.Foreground = this._progressBarThemes[1].Foreground;
    } else {
      progressbar.Background = this._progressBarThemes[0].Background;
      progressbar.BorderBrush = this._progressBarThemes[0].Background;
      progressbar.Foreground = this._progressBarThemes[0].Foreground;
    }
  }

  /// <inheritdoc cref="XLAuth.Models.Abstracts.IReloadableControl.RefreshData(RefreshPart)"/>
  internal void RefreshData(RefreshPart part) {
    this.ViewModel.RefreshData(part);
    this.UpdateProgressBarTheme();
  }

  /// <summary>
  /// Opens the settings
  /// </summary>
  public void OpenSettings() {
    if (this.ViewModel.OpenSettings.CanExecute(parameter: null)) {
      this.ViewModel.OpenSettings.Execute(parameter: null);
    }
  }
}
