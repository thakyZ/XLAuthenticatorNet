using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuth.Models.ViewModel;

namespace XLAuth.Dialogs;

/// <summary>
/// Interaction logic for OTPKeyDialog.xaml
/// </summary>
public partial class OTPKeyDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local"),
   SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
  private OTPKeyDialogViewModel Model
    => (this.DataContext as OTPKeyDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="OTPKeyDialog"/> class
  /// </summary>
  internal OTPKeyDialog() {
    this.InitializeComponent();
    this.DataContext = new OTPKeyDialogViewModel(this);
  }
}
