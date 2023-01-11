using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.PeerToPeer;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Castle.Core.Internal;

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

    internal SettingsControlViewModel Model => DataContext as SettingsControlViewModel;
    private AccountManager _accountManager;

    internal Action LauncherIpUpdated;
    internal Action OtpKeyUpdated;

    private void UpdateLauncherIpAccount(string value) {
      if (_accountManager.CurrentAccount == null ||
      _accountManager.CurrentAccount.Id != $"{value}") {
        var accountToSave = new TotpAccount(value)
        {
          Token = "",
        };

        _accountManager.AddAccount(accountToSave);

        _accountManager.CurrentAccount = accountToSave;
      }
    }

    public string OtpKey {
      get => _accountManager.CurrentAccount.LauncherIpAddress.IsNullOrEmpty() == false ? "Yes" : "No";

      set {
        _accountManager.UpdateToken(_accountManager.CurrentAccount, value);
      }
    }

    internal void CreateAccountManager(ref AccountManager accountManager) {
      _accountManager = accountManager;
    }

    public SettingsControl() {
      InitializeComponent();
      DataContext = new SettingsControlViewModel(ref _accountManager);
      ReloadSettings();
    }

    public void ReloadSettings() {
      if (Model is null || App.Settings is null) {
        throw new NullReferenceException();
      }

      CloseAppAfterSendingBox.IsChecked = App.Settings.CloseApp;

      SettingsDismissed?.Invoke(this, EventArgs.Empty);
    }

    private void SettingsBack_Click(object sender, RoutedEventArgs e) {
      Transitioner.MoveNextCommand.Execute(null, null);
    }
  }
}