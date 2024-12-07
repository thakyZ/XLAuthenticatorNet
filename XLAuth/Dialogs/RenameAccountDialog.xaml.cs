using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// The rename account dialog class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class RenameAccountDialog : UserControl {
  /*
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  private RenameAccountDialogViewModel Model
    => (this.DataContext as RenameAccountDialogViewModel)!;
  */

  /// <summary>
  /// Initializes a new instance of the <see cref="RenameAccountDialog"/> class
  /// </summary>
  internal RenameAccountDialog() {
    this.InitializeComponent();
    this.DataContext = new RenameAccountDialogViewModel(this);
  }
}
