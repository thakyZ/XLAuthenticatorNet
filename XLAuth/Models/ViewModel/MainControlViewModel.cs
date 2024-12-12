using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using OtpNet;
using XLAuth.Config;
using XLAuth.Dialogs;
using XLAuth.Domain;
using XLAuth.Domain.Commands;
using XLAuth.Extensions;
using XLAuth.Models.Abstracts;
using XLAuth.Models.Events;
using XLAuth.Windows;
using NavigationCommands = XLAuth.Domain.Components.Helpers.NavigationCommands;

namespace XLAuth.Models.ViewModel;
/// <summary>
/// The main control view model class
/// </summary>
/// <seealso cref="ViewModelBase{MainControl}"/>
internal sealed class MainControlViewModel : ViewModelBase<MainControl> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(MainWindow) + nameof(this.Title), "XIVLauncher Authenticator");
  /// <summary>
  /// Gets the value of the resend OTP key label
  /// </summary>
  public string ResendOTPKeyLabel => Loc.Localize(nameof(this.ResendOTPKeyLabel), "Resend OTP Key");
  /// <summary>
  /// Gets the value of the current account label
  /// </summary>
  public string CurrentAccountLabel => Loc.Localize(nameof(this.CurrentAccountLabel), "Current Account: ");
  /// <summary>
  /// Gets the value of the label that specifies "Your OTP Key:".
  /// </summary>
  public string YourOTPLabel => Loc.Localize(nameof(this.YourOTPLabel), "Your OTP Key:");
  /// <summary>
  /// Gets or sets the value of the current TOTP
  /// </summary>
  private Totp? CurrentTOTP { get; set; }
  /// <summary>
  /// Gets the value of the current account name
  /// </summary>
  public string CurrentAccountName => App.AccountManager.CurrentAccount?.Name ?? "NULL";
  /// <summary>
  /// The OTP time left
  /// </summary>
  private int _otpTimeLeft;
  /// <summary>
  /// Gets or sets the value of the OTP time left
  /// </summary>
  public int OTPTimeLeft {
    get => this._otpTimeLeft;
    set => this.SetProperty(ref this._otpTimeLeft, value);
  }
  /// <summary>
  /// The OTP value
  /// </summary>
  private string _otpValue = "N/A";
  /// <summary>
  /// Gets or sets the value of the OTP value
  /// </summary>
  public string OTPValue {
    get => this._otpValue;
    private set => this.SetProperty(ref this._otpValue, value);
  }
  /// <summary>
  /// Gets the value of the logo
  /// </summary>
  public ImageSource Logo { get; } = MainControlViewModel.LoadLogo();

  /// <summary>
  /// Gets the value of the message queue
  /// </summary>
  public SnackbarMessageQueue? MessageQueue { get; }

  /// <summary>
  /// Gets or sets the value of the failed to send OTP key count
  /// </summary>
  private static int FailedToSendOTPKeyCount { get; set; }

  /// <summary>
  /// Gets or sets the value of the sending TOTP key
  /// </summary>
  private static bool SendingTOTPKey { get; set; }

  /// <summary>
  /// Gets the value of the open settings
  /// </summary>
  public ICommand OpenSettings => new CommandImpl(() => {
    (this.Parent.ParentWindow as MainWindow)?.SettingsControl.RefreshData(RefreshPart.UpdatePopupContent);
    NavigationCommands.ShowSettingsCommand.Execute(this, null!);
  });

  /// <summary>
  /// Gets the value of the resend OTP key command
  /// </summary>
  public ICommand ResendOTPKeyCommand => new AsyncCommandImpl(this.SendTOTPAsync);

  /// <summary>
  /// Gets the value of the copy OTP key
  /// </summary>
  public ICommand CopyOTPKey => new CommandImpl(() => this.MessageQueue?.Enqueue(Loc.Localize(nameof(this.CopyOTPKey), "Copied OTP key to clipboard")));

  /// <summary>
  /// Initializes a new instance of the <see cref="MainControlViewModel"/> class
  /// </summary>
  /// <param name="mainContent">The main content</param>
  internal MainControlViewModel(MainControl mainContent) : base(mainContent) {
    App.AccountManager.AccountSwitched += this.OnSwitchAccount;
    this.MessageQueue = new SnackbarMessageQueue(new TimeSpan(0, 0, 5), this.Parent.Dispatcher);
  }

#if DEBUG
  public MainControlViewModel() {}
#endif

  private async Task SendTOTPAsync() {
    if (SendingTOTPKey) {
      return;
    }

    SendingTOTPKey = true;
    (OTPKeyResponse response, Exception? exception) = await AccountManager.SendOTPKeyAsync(this._otpValue).ConfigureAwait(false);
    switch (response) {
      case OTPKeyResponse.Success:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Success), "OTP Key sent successfully."));
        break;
      case OTPKeyResponse.Failed:
        this.MessageQueue?.Enqueue(FailedToSendOTPKeyCount > 5
          ? Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Failed) + ">5", "Failed to send OTP Key.\r\nPlease check logs and report the error on GitHub.")
          : Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.Failed), "Failed to send OTP Key.\r\nPlease try again.")
        );
        Logger.Error(exception, "Failed to send OTP Key.");
        FailedToSendOTPKeyCount++;
        break;
      case OTPKeyResponse.CurrentAccountNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.CurrentAccountNull), "Current account is null.\r\nPlease switch accounts or create a new issue on GitHub."));
        Logger.Error(exception, "Current account is null.");
        break;
      case OTPKeyResponse.LauncherIpAddressNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.LauncherIpAddressNull), "Launcher IP address is null.\r\nPlease open settings and specify a valid IP address."));
        Logger.Error(exception, "Launcher IP address is null.");
        break;
      case OTPKeyResponse.OTPValueNull:
        this.MessageQueue?.Enqueue(Loc.Localize(nameof(OTPKeyResponse) + nameof(OTPKeyResponse.OTPValueNull), "OTP Value is null.\r\nPlease open the settings page and specify a valid OTP secret.\r\nIf you think this is an error, please open a new issue on GitHub."));
        Logger.Error(exception, "OTP Value is null.");
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
  }

  /// <inheritdoc cref="XLAuth.Models.Abstracts.IReloadableControl.RefreshData(RefreshPart)"/>
  public override void RefreshData(RefreshPart part) {
    base.RefreshData(part);
    // ReSharper disable once InvertIf
    if (part.Contains(RefreshPart.UpdateOTP) && App.AccountManager.CurrentAccount is not null) {
      this.CurrentTOTP ??= App.AccountManager.CurrentAccount.CreateTOTP();

      if (this.CurrentTOTP is not null) {
        try {
          this.OTPValue = this.CurrentTOTP.ComputeTotp();
          this.OTPTimeLeft = this.CurrentTOTP.RemainingSeconds();
        } catch (IndexOutOfRangeException) {
          this.OTPValue = "N/A";
          this.OTPTimeLeft = 0;
        } catch (Exception ex) {
          Logger.Error(ex, "Failed to calculate OTP value");
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
  /// Triggered w
  /// </summary>
  /// <param name="sender">The sender of this event.</param>
  /// <param name="event">The <see cref="AccountSwitchedEventArgs" /> event args.</param>
  private void OnSwitchAccount(object? sender, AccountSwitchedEventArgs @event) {
    this.NotifyPropertyChanged(nameof(this.CurrentAccountName));
    (this.Parent.ParentWindow as MainWindow)?.SettingsControl.RefreshData(RefreshPart.UpdateAll);
    if (@event.NewAccount is not null) {
      this.CurrentTOTP = @event.NewAccount.CreateTOTP();
    }
  }

  /// <summary>
  /// Loads the app's logo.
  /// </summary>
  /// <returns>The app's logo as a bitmap source</returns>
  private static BitmapSource LoadLogo() {
    var           assembly = Assembly.GetExecutingAssembly();
    using Stream? stream   = assembly.GetManifestResourceStream("logo");
    if (stream is null) {
      return ResourceHelpers.GetBlankBitmap();
    }

    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
    return decoder.Frames[0];
  }

  /// <summary>
  /// Releases the unmanaged resources
  /// </summary>
  protected override void ReleaseUnmanagedResources() {
    App.AccountManager.AccountSwitched -= this.OnSwitchAccount;
    this.MessageQueue?.Dispose();
  }
}
