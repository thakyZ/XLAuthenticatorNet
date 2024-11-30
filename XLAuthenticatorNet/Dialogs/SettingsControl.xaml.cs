using System;
using System.Windows;
using System.Windows.Controls;
using XLAuthenticatorNet.Domain.Components.Helpers;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsControl : UserControl {
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private SettingsControlViewModel ViewModel => (DataContext as SettingsControlViewModel)!;
  /// <summary>
  /// Gets or inits the value of the parent window
  /// </summary>
  public required FrameworkElement ParentWindow { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SettingsControl"/> class
  /// </summary>
  internal SettingsControl() {
    InitializeComponent();
    this.DataContext = new SettingsControlViewModel(this);
  }

  /// <summary>
  /// Reloads the settings
  /// </summary>
  /// <exception cref="NullReferenceException"></exception>
  internal void ReloadSettings() {
    if (this.ViewModel is null) {
      throw new NullReferenceException();
    }

    App.Settings.NotifyReloadTriggered();
    App.AccountManager.NotifyReloadTriggered();
  }

  internal void CloseAndCancelSettings() {
    NavigationCommands.HideSettingsCommand.Execute(this, null);
  }

  /// <summary>
  /// Refreshes the data using the specified update popup content
  /// </summary>
  /// <param name="updatePopupContent">A boolean determining whether to update popup content.</param>
  /// <param name="updateLabels">A boolean determining whether to update all labels.</param>
  internal void RefreshData(bool updatePopupContent = false, bool updateLabels = false) {
    this.ViewModel.RefreshData(updatePopupContent: updatePopupContent, updateLabels: updateLabels);
  }
}