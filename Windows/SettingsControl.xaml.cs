using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using MaterialDesignThemes.Wpf.Transitions;

using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Windows.ViewModel;

namespace XLAuthenticatorNet.Windows {

  /// <summary>
  /// Interaction logic for Settings.xaml
  /// </summary>
  public partial class SettingsControl {

    public event EventHandler SettingsDismissed;

    private SettingsControlViewModel ViewModel => DataContext as SettingsControlViewModel;
    private AccountManager _accountManager;

    public SettingsControl() {
      InitializeComponent();
      DataContext = new SettingsControlViewModel();
      ReloadSettings();
    }

    public void ReloadSettings() {
      if (ViewModel is null || App.Settings is null) {
        throw new NullReferenceException();
      }

      _accountManager = new AccountManager(App.Settings);

      var savedAccount = _accountManager.CurrentAccount;

      if (savedAccount is not null) {
        SwitchAccount(savedAccount, false);
      }

      CloseAppAfterSendingBox.IsChecked = App.Settings.CloseApp;

      SettingsDismissed?.Invoke(this, EventArgs.Empty);
    }

    private void SettingsBack_Click(object sender, RoutedEventArgs e) {
      Transitioner.MoveNextCommand.Execute(null, null);
    }

    private void SwitchAccount(TotpAccount account, bool saveAsCurrent) {
      if (account is null)
        throw new ArgumentNullException(nameof(account));
      if (string.IsNullOrEmpty(_accountManager.CurrentAccount.LauncherIpAddress).Equals(false)) {
        Dispatcher.InvokeAsync(() => ViewModel.SetupOtp(_accountManager.CurrentAccount.LauncherIpAddress, _accountManager.CurrentAccount.Token));
      }
    }
  }
}