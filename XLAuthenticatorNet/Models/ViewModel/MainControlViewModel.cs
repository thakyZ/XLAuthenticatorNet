using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using OtpNet;
using Serilog;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Dialogs;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Domain.Commands;
using XLAuthenticatorNet.Models.Abstracts;
using XLAuthenticatorNet.Models.Events;
using XLAuthenticatorNet.Windows;
using NavigationCommands = XLAuthenticatorNet.Domain.Components.Helpers.NavigationCommands;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The main control view model class
/// </summary>
/// <seealso cref="ViewModelBase{MainControl}"/>
internal class MainControlViewModel : ViewModelBase<MainControl> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(MainWindow) + nameof(Title), "XIVLauncher Authenticator");
  /// <summary>
  /// Gets the value of the resend otp key label
  /// </summary>
  public string ResendOTPKeyLabel => Loc.Localize(nameof(ResendOTPKeyLabel), "Resend OTP Key");
  /// <summary>
  /// Gets the value of the current account label
  /// </summary>
  public string CurrentAccountLabel => Loc.Localize(nameof(CurrentAccountLabel), "Current Account: ");
  /// <summary>
  /// Gets the value of the your otp key label.
  /// </summary>
  public string YourOTPLabel => Loc.Localize(nameof(YourOTPLabel), "Your OTP Key:");
  /// <summary>
  /// Gets or sets the value of the current totp
  /// </summary>
  private Totp? CurrentTotp { get; set; }
  /// <summary>
  /// Gets the value of the current account name
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string CurrentAccountName => App.AccountManager.CurrentAccount?.Name ?? "NULL";
  /// <summary>
  /// The otp time left
  /// </summary>
  private int _otpTimeLeft;
  /// <summary>
  /// Gets or sets the value of the otp time left
  /// </summary>
  public int OTPTimeLeft {
    get => _otpTimeLeft;
    set => this.SetProperty(ref this._otpTimeLeft, value);
  }
  /// <summary>
  /// The otp value
  /// </summary>
  private string _otpValue = "N/A";
  /// <summary>
  /// Gets or sets the value of the otp value
  /// </summary>
  public string OTPValue {
    get => _otpValue;
    private set => this.SetProperty(ref this._otpValue, value);
  }
  /// <summary>
  /// Gets the value of the logo
  /// </summary>
  public ImageSource Logo { get; } = LoadLogo();
  /// <summary>
  /// Gets the value of the message queue
  /// </summary>
  public SnackbarMessageQueue? MessageQueue { get; }
  /// <summary>
  /// Gets or sets the value of the failed to send otp key count
  /// </summary>
  private static int FailedToSendOTPKeyCount { get; set; }
  /// <summary>
  /// Gets or sets the value of the sending totp key
  /// </summary>
  private static bool SendingTOTPKey { get; set; }

  /// <summary>
  /// Gets the value of the open settings
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand OpenSettings => new CommandImpl(() => {
    (this.Parent.ParentWindow as MainWindow)?.SettingsControl.RefreshData(updatePopupContent: true);
    NavigationCommands.ShowSettingsCommand.Execute(this, null!);
  });

  /// <summary>
  /// Gets the value of the resend otp key command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("Performance", "CS1998:Async method lacks 'await' operators and will run synchronously")]
  public ICommand ResendOTPKeyCommand => new AsyncCommandImpl(async () => {
    if (SendingTOTPKey) {
      return;
    }

    SendingTOTPKey = true;
    (OTPKeyResponse response, Exception? exception) = await AccountManager.SendOTPKey(this._otpValue);
    switch (response) {
      case OTPKeyResponse.Success:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Success), "OTP Key sent successfully."));
        break;
      case OTPKeyResponse.Failed:
        this.MessageQueue?.Enqueue(FailedToSendOTPKeyCount > 5
          ? Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Failed) + ">5", "Failed to send OTP Key.\r\nPlease check logs and report the error on GitHub.")
          : Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Failed), "Failed to send OTP Key.\r\nPlease try again.")
        );
        Log.Error(exception, "Failed to send OTP Key.");
        FailedToSendOTPKeyCount++;
        break;
      case OTPKeyResponse.CurrentAccountNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.CurrentAccountNull), "Current account is null.\r\nPlease switch accounts or create a new issue on GitHub."));
        Log.Error(exception, "Current account is null.");
        break;
      case OTPKeyResponse.LauncherIpAddressNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.LauncherIpAddressNull), "Launcher IP address is null.\r\nPlease open settings and specify a valid IP address."));
        Log.Error(exception, "Launcher IP address is null.");
        break;
      case OTPKeyResponse.OTPValueNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.OTPValueNull), "OTP Value is null.\r\nPlease open the settings page and specify a valid OTP secret.\r\nIf you think this is an error, please open a new issue on GitHub."));
        Log.Error(exception, "OTP Value is null.");
        break;
      default:
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual((int)response, 4, nameof(response));
        ArgumentOutOfRangeException.ThrowIfLessThan((int)response, 0, nameof(response));
        break;
    }

    if (exception is not null) {
      CustomMessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    SendingTOTPKey = false;
  });

  /// <summary>
  /// Gets the value of the copy otp key
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand CopyOTPKey => new CommandImpl(() => {
    this.MessageQueue?.Enqueue(Loc.Localize(nameof(CopyOTPKey), "Copied OTP key to clipboard"));
  });

  /// <summary>
  /// Initializes a new instance of the <see cref="MainControlViewModel"/> class
  /// </summary>
  /// <param name="mainContent">The main content</param>
  internal MainControlViewModel(MainControl mainContent) : base(mainContent) {
    App.AccountManager.AccountSwitched += OnSwitchAccount;
    this.MessageQueue = new SnackbarMessageQueue(new TimeSpan(0, 0, 5), this.Parent.Dispatcher);
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public MainControlViewModel() {}
#endif

  /// <summary>
  /// Refreshes the data using the specified update otp
  /// </summary>
  /// <param name="updateOTP">The update otp</param>
  internal void RefreshData(bool updateOTP = false) {
    base.RefreshData();
    // ReSharper disable once InvertIf
    if (updateOTP && App.AccountManager.CurrentAccount is not null) {
      this.CurrentTotp ??= App.AccountManager.CurrentAccount.CreateTOTP();

      if (this.CurrentTotp is not null) {
        try {
          this.OTPValue = this.CurrentTotp.ComputeTotp();
          this.OTPTimeLeft = this.CurrentTotp.RemainingSeconds();
        } catch (IndexOutOfRangeException) {
          this.OTPValue = "N/A";
          this.OTPTimeLeft = 0;
        } catch (Exception ex) {
          Log.Error(ex, "Failed to calculate OTP value");
          this.OTPValue = "N/A";
          this.OTPTimeLeft = 0;
        }
      } else {
        this.OTPValue = "N/A";
        this.OTPTimeLeft = 0;
      }
    }
  }

  /// <summary>
  /// Ons the switch account using the specified  
  /// </summary>
  /// <param name="_">The </param>
  /// <param name="e">The </param>
  private void OnSwitchAccount(object? _, AccountSwitchedEventArgs e) {
    this.NotifyPropertyChanged(nameof(CurrentAccountName));
    (this.Parent.ParentWindow as MainWindow)?.SettingsControl.RefreshData();
    if (e.NewAccount is not null) {
      this.CurrentTotp = e.NewAccount.CreateTOTP();
    }
  }

  /// <summary>
  /// Loads the app's logo.
  /// </summary>
  /// <returns>The app's logo as a bitmap source</returns>
  private static BitmapSource LoadLogo() {
    var assembly = Assembly.GetExecutingAssembly();
    using var stream = assembly.GetManifestResourceStream("logo");
    if (stream is null) return Util.GetBlankBitmap();
    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
    return decoder.Frames[0];
  }

  /// <summary>
  /// Releases the unmanaged resources
  /// </summary>
  protected override void ReleaseUnmanagedResources() {
    App.AccountManager.AccountSwitched -= OnSwitchAccount;
    this.MessageQueue?.Dispose();
  }
}