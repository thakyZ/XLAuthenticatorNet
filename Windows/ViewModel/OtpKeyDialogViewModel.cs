using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLAuthenticatorNet.Windows.ViewModel {

  internal class OtpKeyDialogViewModel : ViewModelBase {
    private string _otpKey;

    public string OtpKey {
      get => _otpKey;
      set => SetProperty(ref _otpKey, value);
    }
  }
}