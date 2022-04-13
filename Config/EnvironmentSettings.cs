using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLAuthenticatorNet.Config {

  internal static class EnvironmentSettings {
    public static bool IsWine => CheckEnvBool("XL_WINEONLINUX");
    public static bool IsDisableUpdates => true;//CheckEnvBool("XL_NOAUTOUPDATE");
    public static bool IsPreRelease => CheckEnvBool("XL_PRERELEASE");
    public static bool IsNoRunas => CheckEnvBool("XL_NO_RUNAS");

    private static bool CheckEnvBool(string var) => bool.Parse(System.Environment.GetEnvironmentVariable(var) ?? "false");
  }
}