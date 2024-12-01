using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Dialogs;

/// <summary>
/// The add account dialog class
/// </summary>
/// <seealso cref="UserControl"/>
public partial class AddAccountDialog : UserControl {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local"),
   SuppressMessage("CodeQuality", "IDE0051:Remove unused private members")]
  private AddAccountDialogViewModel Model
    => (this.DataContext as AddAccountDialogViewModel)!;

  /// <summary>
  /// Initializes a new instance of the <see cref="AddAccountDialog"/> class
  /// </summary>
  internal AddAccountDialog() {
    this.InitializeComponent();
    this.DataContext = new AddAccountDialogViewModel(this);
  }
}