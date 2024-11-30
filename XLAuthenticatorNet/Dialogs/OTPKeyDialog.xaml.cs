using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// Interaction logic for OTPKeyDialog.xaml
/// </summary>
public partial class OTPKeyDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
  private OTPKeyDialogViewModel Model => (this.DataContext as OTPKeyDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="OTPKeyDialog"/> class
  /// </summary>
  internal OTPKeyDialog() {
    InitializeComponent();
    DataContext = new OTPKeyDialogViewModel(this);
  }
}