using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using System.Windows.Input;
using System.Windows.Media;

using MaterialDesignThemes.Wpf;

using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Windows.Dialog;

namespace XLAuthenticatorNet.Windows.ViewModel {

  internal class SettingsControlViewModel : ViewModelBase {
    private AccountManager _accountManager;

    public SettingsControlViewModel(ref AccountManager accountManager) {
    }

    public string LauncherIp;
    public string OtpKey;

    public void CreateAccountManager(ref AccountManager accountManager) => _accountManager = accountManager;

    protected override bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = null) {
      if (propertyName is not null) {
        if (propertyName.Equals("OtpKey")) {
          var _temp = (value as string) != "null" ? true.ToString() : false.ToString();
          SetOTPKey(_temp);
        }
        if (propertyName.Equals("LauncherIP")) {
          var _temp = value as string is null ? _accountManager.CurrentAccount.LauncherIpAddress : value as string;
          SetIP(_temp ?? _accountManager.CurrentAccount.LauncherIpAddress);
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
      if (StartCheckIP(arg)) {
        LauncherIPText = arg;
        LauncherIPColor = new SolidColorBrush(Colors.LimeGreen);
      } else {
        LauncherIPText = arg;
        LauncherIPColor = new SolidColorBrush(Colors.Red);
      }
    }

    private void SetIP(string arg, bool updated) {
      if (updated) {
        if (StartCheckIP(arg)) {
          LauncherIPText = arg;
          LauncherIPColor = new SolidColorBrush(Colors.LimeGreen);
        } else {
          LauncherIPText = arg;
          LauncherIPColor = new SolidColorBrush(Colors.Red);
        }
      }
    }

    internal bool StartCheckIP(string args) {
      var checkIP = CheckIP(args);
      return checkIP.Result;
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
        DataContext = new LauncherIpDiagViewModel(ref _accountManager)
      };

      var result = await DialogHost.Show(view, "RootDialog", SetLauncherIpClosingEventHandler);
    }

    private void SetLauncherIpClosingEventHandler(object sender, DialogClosingEventArgs eventArgs) {
      if (eventArgs.Parameter is not Array)
        return;
      if (!Equals((eventArgs.Parameter as Array)?.GetValue(0), true))
        return;

      SetIP((eventArgs.Parameter as Array)?.GetValue(1) as string ?? "null");
    }
  }
}