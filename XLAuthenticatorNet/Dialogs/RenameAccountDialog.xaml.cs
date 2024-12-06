using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// The rename account dialog class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class RenameAccountDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local"),
   SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
  private RenameAccountDialogViewModel Model
    => (this.DataContext as RenameAccountDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="RenameAccountDialog"/> class
  /// </summary>
  internal RenameAccountDialog() {
    this.InitializeComponent();
    this.DataContext = new RenameAccountDialogViewModel(this);
  }
}
