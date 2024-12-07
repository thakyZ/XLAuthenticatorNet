using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// Interaction logic for LauncherIPDialog.xaml
/// </summary>
public partial class LauncherIPDialog : UserControl {
  /*
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  private LauncherIPDialogViewModel Model
    => (this.DataContext as LauncherIPDialogViewModel)!;
  */

  /// <summary>
  /// Initializes a new instance of the <see cref="LauncherIPDialog"/> class
  /// </summary>
  internal LauncherIPDialog() {
    this.InitializeComponent();
    this.DataContext = new LauncherIPDialogViewModel(this);
  }
}
