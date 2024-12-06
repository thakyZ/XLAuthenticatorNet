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
/// The rename account dialog view model class
/// </summary>
/// <seealso cref="ViewModelBase{RenameAccountDialog}"/>
internal sealed class RenameAccountDialogViewModel : ViewModelBase<RenameAccountDialog> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(RenameAccountDialog) + nameof(this.Title), "New Account");
  /// <summary>
  /// Gets the value of the account name label
  /// </summary>
  public string AccountNameLabel => Loc.Localize(nameof(this.AccountNameLabel), "Name:");
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
  /// Gets the value of the submit rename account dialog
  /// </summary>
  public ICommand SubmitRenameAccountDialog => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.OK, this.AccountNameValue), target: null));
  /// <summary>
  /// Gets the value of the cancel rename account dialog
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public ICommand CancelRenameAccountDialog => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.Cancel), target: null));

  /// <summary>
  /// Initializes a new instance of the <see cref="RenameAccountDialogViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal RenameAccountDialogViewModel(RenameAccountDialog parent) : base(parent) {}

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public RenameAccountDialogViewModel() {}
#endif
}
