using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XLAuthenticatorNet.Config {

  public interface IAuthSettingsV1 {

    #region Launcher Setting

    bool CloseApp { get; set; }
    string CurrentAccountId { get; set; }

    #endregion Launcher Setting
  }
}