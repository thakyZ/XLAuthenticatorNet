using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using MaterialDesignThemes.Wpf;
using Serilog;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// The main control class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class MainControl : UserControl {
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private MainControlViewModel ViewModel => (this.DataContext as MainControlViewModel)!;
  /// <summary>
  /// Gets or inits the value of the parent window
  /// </summary>
  public required FrameworkElement ParentWindow { get; init; }

  /// <summary>
  /// The background
  /// </summary>
  private readonly Dictionary<int, (Brush Foreground, Brush Background)> _progressBarThemes = new Dictionary<int, (Brush Foreground, Brush Background)>(2);

  /// <summary>
  /// Initializes a new instance of the <see cref="MainControl"/> class
  /// </summary>
  internal MainControl() {
    InitializeComponent();
    this.DataContext = new MainControlViewModel(this);
  }

  /// <summary>
  /// Updates the progress bar theme
  /// </summary>
  private void UpdateProgressBarTheme() {
    try {
      Theme? theme = App.GetTheme();
      if (theme is null || _progressBarThemes.Count == 2) {
        return;
      }
      Log.Debug("Update progress bar themes.");
      this._progressBarThemes[0] = (Foreground: new SolidColorBrush(theme.PrimaryDark.ForegroundColor ?? theme.PrimaryDark.Color),
                                    Background: new SolidColorBrush(theme.PrimaryLight.ForegroundColor ?? theme.PrimaryLight.Color));
      this._progressBarThemes[1] = (Foreground: new SolidColorBrush(MaterialDesignColors.Recommended.RedSwatch.Red700),
                                    Background: new SolidColorBrush(MaterialDesignColors.Recommended.RedSwatch.Red200));
    } catch (Exception e) {
      Log.Error(e, "Failed to set progress bar theme.");
    }
  }

  /// <summary>
  /// Otps the time left on value changed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void OTPTimeLeft_OnValueChanged(object? sender, RoutedPropertyChangedEventArgs<double> e) {
    if (sender is not ProgressBar progressbar || _progressBarThemes.Keys.Count != 2) {
      return;
    }

    if (e.NewValue < 8) {
      progressbar.Background = _progressBarThemes[1].Background;
      progressbar.BorderBrush = _progressBarThemes[1].Background;
      progressbar.Foreground = _progressBarThemes[1].Foreground;
    } else {
      progressbar.Background = _progressBarThemes[0].Background;
      progressbar.BorderBrush = _progressBarThemes[0].Background;
      progressbar.Foreground = _progressBarThemes[0].Foreground;
    }
  }

  /// <summary>
  /// Refreshes the data using the specified update otp
  /// </summary>
  /// <param name="updateOTP">The update otp</param>
  internal void RefreshData(bool updateOTP = false) {
    this.ViewModel.RefreshData(updateOTP);
    this.UpdateProgressBarTheme();
  }

  /// <summary>
  /// Opens the settings
  /// </summary>
  public void OpenSettings() {
    if (this.ViewModel.OpenSettings.CanExecute(null)) {
      this.ViewModel.OpenSettings.Execute(null);
    }
  }
}