using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

using XLAuthenticatorNet;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Windows.Dialog;

namespace XLAuthenticatorNet.Windows.ViewModel {

  internal class SettingsControlViewModel : ViewModelBase {
    public AccountManager AccountManager { get; private set; } = new(App.Settings);

    private string _launcherIp;

    public string LauncherIp {
      get => _launcherIp;

      set {
        _launcherIp = value;
        SetProperty(ref _launcherIp, value, "LauncherIP");
      }
    }

    private string _otpKey;

    public string OtpKey {
      get => AccountManager.CurrentAccount.Token != "null" ? "Yes" : "No";

      set {
        AccountManager.CurrentAccount.Token = value;
        SetProperty(ref _otpKey, value, "OtpKey");
      }
    }

    public SettingsControlViewModel() {
    }

    public async Task SetupOtp(string launcherIp, string token) {
      if (AccountManager.CurrentAccount != null && AccountManager.CurrentAccount.LauncherIpAddress.Equals(launcherIp) &&
          AccountManager.CurrentAccount.Token != token &&
          AccountManager.CurrentAccount.SavePassword) {
        AccountManager.UpdateToken(AccountManager.CurrentAccount, token);
      }

      if (AccountManager.CurrentAccount == null || AccountManager.CurrentAccount.Id != $"{launcherIp}") {
        var accountToSave = new TotpAccount(launcherIp) {
          Token = token
        };

        AccountManager.AddAccount(accountToSave);

        AccountManager.CurrentAccount = accountToSave;
      }

      LauncherIp = launcherIp;
      OtpKey = token;

      LauncherIPText = LauncherIp;
      IsRegisteredText = OtpKey;
    }

    protected override bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null) {
      if (propertyName is not null) {
        if (propertyName.Equals("OtpKey")) {
          var _temp = (value as string) != "null" ? true.ToString() : false.ToString();
          SetOTPKey(_temp);

          AccountManager.AddAccount(new TotpAccount(LauncherIp) {
            Token = OtpKey
          });
        }
        if (propertyName.Equals("LauncherIP")) {
          var _temp = value as string is null ? AccountManager.CurrentAccount.LauncherIpAddress : value as string;
          SetIP(_temp ?? AccountManager.CurrentAccount.LauncherIpAddress);

          AccountManager.AddAccount(new TotpAccount(LauncherIp) {
          });
        }
      }

      return base.SetProperty(ref member, value, propertyName);
    }

    private void SetOTPKey(string value) {
      if (value.Equals("Yes")) {
        IsRegisteredText = "Yes";
        IsRegisteredColor = new SolidColorBrush(Colors.LimeGreen);
      } else {
        IsRegisteredText = "No";
        IsRegisteredColor = new SolidColorBrush(Colors.Red);
      }
    }

    private void SetIP(string arg) {
      var checkedIp = CheckIP(arg);
      if (checkedIp.Result) {
        LauncherIPText = arg;
        LauncherIPColor = new SolidColorBrush(Colors.LimeGreen);
      } else {
        LauncherIPText = arg;
        LauncherIPColor = new SolidColorBrush(Colors.Red);
      }
    }

    private async Task<bool> StartCheckIP(string args) {
      var checkIP = await CheckIP(args);
      return checkIP;
    }

    private static Task<bool> CheckIP(string args) {
      var tcs = new TaskCompletionSource<bool>();
      Ping ping = new Ping();
      try {
        Console.WriteLine($"Checking IP: {args}");
        IPAddress address = IPAddress.Parse(args);
        PingReply pong = ping.Send(address);
        tcs.SetResult(pong.Status.Equals(IPStatus.Success));
        Console.WriteLine($"Ping Completed: {tcs.Task.Result}");
        return tcs.Task;
      } catch (Exception e) {
        Console.WriteLine($"Error: Failed to ping {args}");
        Console.Write($"{e.Message}");
        tcs.SetException(e);
        return tcs.Task;
      }
    }

    public string LauncherIPText { get; private set; }
    public Brush LauncherIPColor { get; private set; }

    public string IsRegisteredText { get; private set; }
    public Brush IsRegisteredColor { get; private set; }

    public ICommand SetOtpKeyDialogCommand => new CommandImpl(SetOtpKeyDialog);

    private async void SetOtpKeyDialog(object commandParameter) {
      var view = new OtpKeyDialog() {
        DataContext = new OtpKeyDialogViewModel()
      };

      var result = await DialogHost.Show(view, "RootDialog", SetOtpKeyClosingEventHandler);
    }

    private void SetOtpKeyClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
      // Do nothing...
    }

    public ICommand SetLauncherIpDialogCommand => new CommandImpl(SetLauncherIpDialog);

    private async void SetLauncherIpDialog(object commandParameter) {
      var view = new LauncherIpDialog() {
        DataContext = new LauncherIpDiagViewModel()
      };

      var result = await DialogHost.Show(view, "RootDialog", SetLauncherClosingEventHandler);
    }

    private void SetLauncherClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
      if (eventArgs.Parameter is not Array)
        return;
      if (!Equals((eventArgs.Parameter as Array)?.GetValue(0), true))
        return;

      LauncherIp = (eventArgs.Parameter as Array)?.GetValue(1) as string ?? "null";
    }
  }
}