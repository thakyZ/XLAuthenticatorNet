using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using XLAuthenticatorNet.Config;

namespace XLAuthenticatorNet.Windows.ViewModel {

  public class MainWindowViewModel : ViewModelBase {
    private int timerValue;
    private int codeValue;
    public Action Activate;
    public Action Hide;

    public MainWindowViewModel() {
    }
  }
}