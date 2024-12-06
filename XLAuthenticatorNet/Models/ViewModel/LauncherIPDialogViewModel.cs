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
/// The launcher IP dialog view model class
/// </summary>
/// <seealso cref="ViewModelBase{LauncherIPDialog}"/>
internal sealed class LauncherIPDialogViewModel : ViewModelBase<LauncherIPDialog> {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(LauncherIPDialog) + nameof(this.Title), "Enter XIVLauncher IP");
  /// <summary>
  /// Gets the value of the submit button label
  /// </summary>
  public string SubmitButtonLabel => Loc.Localize(nameof(this.SubmitButtonLabel), "OK");
  /// <summary>
  /// Gets the value of the cancel button label
  /// </summary>
  public string CancelButtonLabel => Loc.Localize(nameof(this.CancelButtonLabel), "Cancel");
  /// <summary>
  /// Gets or sets the value of the launcher IP value command.
  /// </summary>
  public string? LauncherIPValue { get; set; }
  /// <summary>
  /// Gets the value of the submit launcher IP dialog command.
  /// </summary>
  public ICommand SubmitLauncherIPDialog
    => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.OK, this.LauncherIPValue), target: null));
  /// <summary>
  /// Gets the value of the cancel launcher IP dialog command.
  /// </summary>
  [SuppressMessage("Performance", "CA1822:Mark members as static"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
  public ICommand CancelLauncherIPDialog
    => new CommandImpl(() => DialogHost.CloseDialogCommand.Execute(new DialogResult<string>(MessageBoxResult.Cancel), target: null));

  /// <summary>
  /// Initializes a new instance of the <see cref="LauncherIPDialogViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal LauncherIPDialogViewModel(LauncherIPDialog parent) : base(parent) {
    this.LauncherIPValue = App.AccountManager.CurrentAccount?.LauncherIpAddress?.ToString();
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public LauncherIPDialogViewModel() {}
#endif
}
