using System.Windows.Controls;

using XLAuthenticatorNet.Config;

namespace XLAuthenticatorNet.Windows.Dialog {
  /// <summary>
  /// Interaction logic for OtpKeyDialog.xaml
  /// </summary>
  public partial class OtpKeyDialog : UserControl {

    public OtpKeyDialog(AccountManager accountManager) {
      InitializeComponent();
      SubmitButton.Click += SubmitButton_Click;
      CancelButton.Click += CancelButton_Click;
      InputTextBox.Text = accountManager.CurrentAccount.LauncherIpAddress;
    }

    private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
      CancelButton.CommandParameter = new object[] { false, InputTextBox.Text };
    }

    private void SubmitButton_Click(object sender, System.Windows.RoutedEventArgs e) {
      SubmitButton.CommandParameter = new object[] { true, InputTextBox.Text };
    }
  }
}
