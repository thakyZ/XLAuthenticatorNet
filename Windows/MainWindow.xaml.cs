using System;
using System.Windows;

using XLAuthenticatorNet.Windows.ViewModel;

using XLAuthenticatorNet;
using XLAuthenticatorNet.Config;
using NuGet;
using Castle.Core.Internal;

namespace XLAuthenticatorNet.Windows {

  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private AccountManager _accountManager;
    private MainWindowViewModel Model => this.DataContext as MainWindowViewModel;

    public MainWindow() {
      InitializeComponent();
      this.DataContext = new MainWindowViewModel();

      Model.Activate += () => this.Dispatcher.Invoke(() => {
        this.Show();
        this.Activate();
        this.Focus();
      });

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

      if (App.SetupDevice)
        OpenSettingsWindow();

      if (EnvironmentSettings.IsWine)
        Title += " - Wine on Linux";

#if DEBUG
      Title += " - Debugging";
#endif
    }

    private void OpenSettingsWindow() {
      MainWindowTransitioner.SelectedIndex = 0;
    }

    public void Initialize() {
      Console.WriteLine("[INF] MainWindow initialized.");

      _accountManager = new AccountManager(App.Settings);

      var savedAccount = _accountManager.CurrentAccount;

      if (savedAccount != null)
        SwitchAccount(savedAccount, false);

      SettingsControl.CreateAccountManager(ref _accountManager);
      SettingsControl.ReloadSettings();

      Show();
      Activate();
    }

    private void SwitchAccount(TotpAccount account, bool saveAsCurrent) {
      SettingsControl.Model.LauncherIp = account.LauncherIpAddress;
      SettingsControl.Model.OtpKey = HandleOtpKey(account.Token);

      if (saveAsCurrent) {
        _accountManager.CurrentAccount = account;
      }
    }

    private string HandleOtpKey(string key) {
      return key.IsNullOrEmpty() ? "No" : "Yes";    
    }

    private void OnAccountSwitchedEventHandler(object sender, TotpAccount e) {
      SwitchAccount(e, true);
    }

    private void ResendOTPKey_Click(object sender, RoutedEventArgs e) {
    }

    private void OpenSettings_Click(object sender, RoutedEventArgs e) {
      OpenSettingsWindow();
    }

    private void SettingsControl_OnSettingsDismissed(object sender, EventArgs e) {
      if (App.Settings is null) {
        throw new NullReferenceException();
      }
    }
  }
}