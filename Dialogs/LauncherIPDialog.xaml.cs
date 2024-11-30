using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// Interaction logic for LauncherIPDialog.xaml
/// </summary>
public partial class LauncherIPDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private LauncherIPDialogViewModel Model => (this.DataContext as LauncherIPDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="LauncherIPDialog"/> class
  /// </summary>
  internal LauncherIPDialog() {
    InitializeComponent();
    this.DataContext = new LauncherIPDialogViewModel(this);
  }
}