using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using XLAuth.Config;
using XLAuth.Dialogs;
using XLAuth.Models.ViewModel;

namespace XLAuth.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window {
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private MainWindowViewModel ViewModel => (this.DataContext as MainWindowViewModel)!;
  /// <summary>
  /// Gets the value of the settings control
  /// </summary>
  internal SettingsControl SettingsControl { get; }
  /// <summary>
  /// Gets the value of the main content
  /// </summary>
  internal MainControl MainContent { get; }
  /// <summary>
  /// The timer
  /// </summary>
  private readonly DispatcherTimer _timer;

  /// <summary>
  /// Initializes a new instance of the <see cref="MainWindow"/> class
  /// </summary>
  /// <exception cref="Exception">Failed to load main window</exception>
  internal MainWindow() {
    this.SettingsControl = new SettingsControl { ParentWindow = this };
    this.MainContent = new MainControl { ParentWindow = this };
    this.InitializeComponent();
    this.DataContext = new MainWindowViewModel(this);
#if !XL_NOAUTOUPDATE
      Title += " v" + Util.GetAssemblyVersion();
#else
    this.Title += " " + XLAuth.Support.Util.GetGitHash();
#endif
#if !XL_NOAUTOUPDATE
      if (EnvironmentSettings.IsDisableUpdates)
#endif
    {
      this.Title += " - UNSUPPORTED VERSION - NO UPDATES - COULD DO BAD THINGS";
    }

    this._timer = new DispatcherTimer(DispatcherPriority.Background) {
      Interval = TimeSpan.FromSeconds(1),
      IsEnabled = false,
    };

    this._timer.Tick += this.DispatcherTimer_Tick;
    this._timer.Start();

    if (EnvironmentSettings.IsWine) {
      this.Title += " - Wine on Linux";
    }

#if DEBUG
    this.Title += " - Debugging";
#endif
    Logger.Information("MainWindow initializing...");
    if (this.ViewModel is null) {
      throw new Exception("Failed to load main window");
    }

    TOTPAccount? savedAccount = App.AccountManager.CurrentAccount;
    if (savedAccount is not null) {
      App.AccountManager.SwitchAccount(savedAccount, saveAsCurrent: false);
    }

    this.SettingsControl.ReloadSettings();
    this.Show();
    this.Activate();
  }

  /// <summary>
  /// Dispatchers the timer tick using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="event">The </param>
  private void DispatcherTimer_Tick(object? sender, EventArgs @event) {
    this.MainContent.RefreshData(updateOTP: true);
  }

  /// <summary>
  /// Main the window on closing using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="event">The </param>
  private void MainWindow_OnClosing(object? sender, CancelEventArgs @event) {
    this.ViewModel.Dispose();
    this.Hide();
  }
}
