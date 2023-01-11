using System.Windows.Controls;

using Castle.DynamicProxy.Contributors;

using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Windows.ViewModel;

namespace XLAuthenticatorNet.Windows.Dialog {

  /// <summary>
  /// Interaction logic for LaucherIpDiag.xaml
  /// </summary>
  public partial class LauncherIpDialog : UserControl {
    public string Result { get; private set; }

    public LauncherIpDialog() {
      InitializeComponent();
      SubmitButton.Click += SubmitButton_Click;
      CancelButton.Click += CancelButton_Click;
    }

    private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e) {
      CancelButton.CommandParameter = new object[ ] { false, InputTextBox.Text };
    }

    private void SubmitButton_Click(object sender, System.Windows.RoutedEventArgs e) {
      SubmitButton.CommandParameter = new object[ ] { true, InputTextBox.Text };
    }
  }
}