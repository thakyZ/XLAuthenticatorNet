using System.Diagnostics.CodeAnalysis;
using System.Security;
using System.Windows;
using System.Windows.Input;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using XLAuthenticatorNet.Dialogs;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Domain.Commands;
using XLAuthenticatorNet.Models.Abstracts;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The OTP key dialog view model class
/// </summary>
/// <seealso cref="ViewModelBase{OTPKeyDialog}"/>
internal sealed class OTPKeyDialogViewModel : ViewModelBase<OTPKeyDialog> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(OTPKeyDialog) + nameof(this.Title), "Enter OTP Secret");
  /// <summary>
  /// Gets the value of the submit button label
  /// </summary>
  public string SubmitButtonLabel => Loc.Localize(nameof(this.SubmitButtonLabel), "OK");
  /// <summary>
  /// Gets the value of the cancel button label
  /// </summary>
  public string CancelButtonLabel => Loc.Localize(nameof(this.CancelButtonLabel), "Cancel");
  /// <summary>
  /// Gets or sets the value of the OTP key value
  /// </summary>
  public SecureString? OTPKeyValue { get; set; }
  /// <summary>
  /// Gets the value of the submit OTP key dialog
  /// </summary>
  public ICommand SubmitOTPKeyDialog => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<SecureString>(MessageBoxResult.OK, this.OTPKeyValue), target: null));
  /// <summary>
  /// Gets the value of the cancel OTP key dialog
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public ICommand CancelOTPKeyDialog => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<SecureString>(MessageBoxResult.Cancel), target: null));

  /// <summary>
  /// Initializes a new instance of the <see cref="OTPKeyDialogViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal OTPKeyDialogViewModel(OTPKeyDialog parent) : base(parent) {
    this.OTPKeyValue = App.AccountManager.CurrentAccount?.Token;
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public OTPKeyDialogViewModel() {}
#endif
}