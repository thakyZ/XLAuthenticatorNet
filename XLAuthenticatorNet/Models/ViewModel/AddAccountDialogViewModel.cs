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
/// The add account dialog view model class
/// </summary>
/// <seealso cref="ViewModelBase{AddAccountDialog}"/>
internal sealed class AddAccountDialogViewModel : ViewModelBase<AddAccountDialog> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(AddAccountDialog) + nameof(this.Title), "New Account");

  /// <summary>
  /// Gets the value of the account name label
  /// </summary>
  public string AccountNameLabel => Loc.Localize(nameof(this.AccountNameLabel), "Name: ");

  /// <summary>
  /// Gets the value of the submit button label
  /// </summary>
  public string SubmitButtonLabel { get; } = Loc.Localize(nameof(SubmitButtonLabel), "OK");

  /// <summary>
  /// Gets the value of the cancel button label
  /// </summary>
  public string CancelButtonLabel { get; } = Loc.Localize(nameof(CancelButtonLabel), "Cancel");

  /// <summary>
  /// Gets or sets the value of the account name value
  /// </summary>
  public string AccountNameValue { get; set; } = string.Empty;

  /// <summary>
  /// Gets the value of the submit add account dialog
  /// </summary>
  public ICommand SubmitAddAccountDialog
    => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.OK, this.AccountNameValue), target: null));

  /// <summary>
  /// Gets the value of the cancel add account dialog
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand CancelAddAccountDialog
    => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.Cancel), target: null));

  /// <summary>
  /// Initializes a new instance of the <see cref="AddAccountDialogViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal AddAccountDialogViewModel(AddAccountDialog parent) : base(parent) {}

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public AddAccountDialogViewModel() {}
#endif
}
