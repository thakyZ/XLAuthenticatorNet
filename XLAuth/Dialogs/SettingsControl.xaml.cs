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

  internal void CancelSettingChanges() {
    NavigationCommands.HideSettingsCommand.Execute(this, target: null);
  }

  /// <inheritdoc cref="XLAuth.Models.Abstracts.IReloadableControl.RefreshData(RefreshPart)"/>
  internal void RefreshData(RefreshPart part) {
    this.ViewModel.RefreshData(part);
  }
}
