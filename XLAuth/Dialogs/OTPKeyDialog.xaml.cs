using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// Interaction logic for OTPKeyDialog.xaml
/// </summary>
public partial class OTPKeyDialog : UserControl {
  /*
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  private OTPKeyDialogViewModel Model
   => (this.DataContext as OTPKeyDialogViewModel)!;
  */

  /// <summary>
  /// Initializes a new instance of the <see cref="OTPKeyDialog"/> class
  /// </summary>
  internal OTPKeyDialog() {
    this.InitializeComponent();
    this.DataContext = new OTPKeyDialogViewModel(this);
  }
}
