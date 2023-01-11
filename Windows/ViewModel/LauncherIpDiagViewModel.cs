using System.Runtime.CompilerServices;

using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Windows.Dialog;

namespace XLAuthenticatorNet.Windows.ViewModel {

  public class LauncherIpDiagViewModel : ViewModelBase {
    private AccountManager _accountManager = null;
    public LauncherIpDiagViewModel(ref AccountManager accountManager) => _accountManager = accountManager;

    public string LauncherIP => _accountManager.CurrentAccount.LauncherIpAddress;
  }
}