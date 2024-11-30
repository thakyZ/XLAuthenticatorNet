using System.Diagnostics.CodeAnalysis;
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
/// The delete account dialog view model class
/// </summary>
/// <seealso cref="ViewModelBase{DeleteAccountDialog}"/>
internal class DeleteAccountDialogViewModel : ViewModelBase<DeleteAccountDialog> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(DeleteAccountDialog) + nameof(Title), "New Account");
  /// <summary>
  /// Gets the value of the delete message label
  /// </summary>
  public string DeleteMessageLabel { get; }
  /// <summary>
  /// Gets the value of the submit button label
  /// </summary>
  public string SubmitButtonLabel => Loc.Localize(nameof(SubmitButtonLabel), "Yes");
  /// <summary>
  /// Gets the value of the cancel button label
  /// </summary>
  public string CancelButtonLabel => Loc.Localize(nameof(CancelButtonLabel), "No");
  /// <summary>
  /// Gets the value of the submit delete account dialog
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public ICommand SubmitDeleteAccountDialog => new CommandImpl(() => {
    DialogHost.CloseDialogCommand.Execute(new DialogResult<bool>(MessageBoxResult.Yes, true), null);
  });
  /// <summary>
  /// Gets the value of the cancel delete account dialog
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public ICommand CancelDeleteAccountDialog => new CommandImpl(() => {
    DialogHost.CloseDialogCommand.Execute(new DialogResult<bool>(MessageBoxResult.No), null);
  });

  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteAccountDialogViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal DeleteAccountDialogViewModel(DeleteAccountDialog parent) : base(parent) {
    this.DeleteMessageLabel = string.Format(Loc.Localize(nameof(DeleteMessageLabel), "Are you sure you want to delete this account?\n\"{0}\""), App.AccountManager.CurrentAccount?.Name ?? "null");
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public DeleteAccountDialogViewModel() {
    this.DeleteMessageLabel = string.Format(Loc.Localize(nameof(DeleteMessageLabel), "Are you sure you want to delete this account?\n\"{0}\""), App.AccountManager.CurrentAccount?.Name ?? "null");
  }
#endif
}