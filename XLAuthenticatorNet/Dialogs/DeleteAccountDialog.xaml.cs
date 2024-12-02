using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// The delete account dialog class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class DeleteAccountDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private DeleteAccountDialogViewModel Model => (this.DataContext as DeleteAccountDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="DeleteAccountDialog"/> class
  /// </summary>
  internal DeleteAccountDialog() {
    InitializeComponent();
    this.DataContext = new DeleteAccountDialogViewModel(this);
  }
}