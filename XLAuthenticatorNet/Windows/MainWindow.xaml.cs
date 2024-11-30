using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using Serilog;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Dialogs;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Windows {
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
      InitializeComponent();
      this.DataContext = new MainWindowViewModel(this);
#if !XL_NOAUTOUPDATE
      Title += " v" + Util.GetAssemblyVersion();
#else
      Title += " " + Util.GetGitHash();
#endif
#if !XL_NOAUTOUPDATE
      if (EnvironmentSettings.IsDisableUpdates)
#endif
      {
        Title += " - UNSUPPORTED VERSION - NO UPDATES - COULD DO BAD THINGS";
      }

      _timer = new DispatcherTimer(DispatcherPriority.Background) {
        Interval = TimeSpan.FromSeconds(1), IsEnabled = false,
      };
      _timer.Tick += DispatcherTimer_Tick;
      _timer.Start();

      if (EnvironmentSettings.IsWine) {
        Title += " - Wine on Linux";
      }

#if DEBUG
      Title += " - Debugging";
#endif
      Log.Information("MainWindow initializing...");
      if (this.ViewModel is null) {
        throw new Exception("Failed to load main window");
      }

      TotpAccount? savedAccount = App.AccountManager.CurrentAccount;
      if (savedAccount is not null) {
        App.AccountManager.SwitchAccount(savedAccount, false);
      }

      this.SettingsControl.ReloadSettings();
      Show();
      Activate();
    }

    /// <summary>
    /// Dispatchers the timer tick using the specified sender
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The </param>
    private void DispatcherTimer_Tick(object? sender, EventArgs e) {
      this.MainContent.RefreshData(updateOTP: true);
    }

    /// <summary>
    /// Main the window on closing using the specified sender
    /// </summary>
    /// <param name="sender">The sender</param>
    /// <param name="e">The </param>
    private void MainWindow_OnClosing(object? sender, CancelEventArgs e) {
      this.ViewModel.Dispose();
      this.Hide();
    }
  }
}