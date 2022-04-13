using System.Runtime.CompilerServices;

namespace XLAuthenticatorNet.Windows.ViewModel {

  public class LauncherIpDiagViewModel : ViewModelBase {
    private string _launcherIp;

    public string LauncherIP {
      get => _launcherIp;
      set => SetProperty(ref _launcherIp, value);
    }
  }
}