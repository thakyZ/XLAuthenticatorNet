using System;
using AdysTech.CredentialManager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.ComponentModel;
using System.Net;

namespace XLAuthenticatorNet.Config {

  public class TotpAccount {

    [JsonIgnore]
    public string Id => $"{LauncherIpAddress}-{CloseOnSend}";

    public override string ToString() => Id;

    public string LauncherIpAddress { get; private set; }
    public bool CloseOnSend { get; private set; }

    [JsonIgnore]
    public string Token {
      get {
        var credentials = CredentialManager.GetCredentials($"XIVQuickLauncherAuth-{LauncherIpAddress.ToLower()}");
        return credentials != null ? credentials.Password : string.Empty;
      }

      set {
        try {
          CredentialManager.RemoveCredentials($"XIVQuickLauncherAuth-{LauncherIpAddress.ToLower()}");
        } catch (Win32Exception) {
          // ignored
        }

        CredentialManager.SaveCredentials($"XIVQuickLauncherAuth-{LauncherIpAddress.ToLower()}", new NetworkCredential {
          UserName = LauncherIpAddress,
          Password = value
        });
      }
    }

    public bool SavePassword => true;

    public TotpAccount(string launcherIpAddress) {
      LauncherIpAddress = launcherIpAddress.ToLower();
    }
  }
}