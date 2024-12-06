using System;
using System.Windows;
using System.Windows.Controls;
using XLAuth.Domain.Components.Helpers;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// Interaction logic for Settings.xaml
/// </summary>
public partial class SettingsControl : UserControl {
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private SettingsControlViewModel ViewModel => (this.DataContext as SettingsControlViewModel)!;
  /// <summary>
  /// Gets or inits the value of the parent window
  /// </summary>
  public required FrameworkElement ParentWindow { get; init; }

  /// <summary>
  /// Initializes a new instance of the <see cref="SettingsControl"/> class
  /// </summary>
  internal SettingsControl() {
    this.InitializeComponent();
    this.DataContext = new SettingsControlViewModel(this);
  }

  /// <summary>
  /// Reloads the settings
  /// </summary>
  /// <exception cref="NullReferenceException"></exception>
  internal void ReloadSettings() {
    if (this.ViewModel is null) {
      throw new InvalidOperationException();
    }

    App.Settings.NotifyReloadTriggered();
    App.AccountManager.OnReloadTriggered();
  }

  internal void CloseAndCancelSettings() {
    NavigationCommands.HideSettingsCommand.Execute(this, target: null);
  }

  /// <summary>
  /// Refreshes the data using the specified update pop-up content
  /// </summary>
  /// <param name="updatePopupContent">A boolean determining whether to update pop-up content.</param>
  /// <param name="updateLabels">A boolean determining whether to update all labels.</param>
  internal void RefreshData(bool updatePopupContent = false, bool updateLabels = false) {
    this.ViewModel.RefreshData(updatePopupContent: updatePopupContent, updateLabels: updateLabels);
  }
}
