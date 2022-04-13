using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XLAuthenticatorNet {

  public static class Util {

    public static void ShowError(string message, string caption, [CallerMemberName] string callerName = "",
        [CallerLineNumber] int callerLineNumber = 0) {
      MessageBox.Show($"{message}\n\n{callerName} L{callerLineNumber}", caption, MessageBoxButton.OK,
          MessageBoxImage.Error);
    }

    /// <summary>
    ///     Gets the git hash value from the assembly
    ///     or null if it cannot be found.
    /// </summary>
    public static string GetGitHash() {
      var asm = typeof(Util).Assembly;
      var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
      return attrs.FirstOrDefault(a => a.Key == "GitHash")?.Value;
    }

    /// <summary>
    ///     Gets the build origin from the assembly
    ///     or null if it cannot be found.
    /// </summary>
    public static string GetBuildOrigin() {
      var asm = typeof(Util).Assembly;
      var attrs = asm.GetCustomAttributes<AssemblyMetadataAttribute>();
      return attrs.FirstOrDefault(a => a.Key == "BuildOrigin")?.Value;
    }

    public static string GetAssemblyVersion() {
      var assembly = Assembly.GetExecutingAssembly();
      var auth = FileVersionInfo.GetVersionInfo(assembly.Location);
      return auth.FileVersion;
    }

    public static long GetUnixMillis() {
      return (long)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
  }

  public class Paths {
    public static string RoamingPath = Path.GetDirectoryName(typeof(Paths).Assembly.Location);

    public static string ResourcesPath = Path.Combine(Path.GetDirectoryName(typeof(Paths).Assembly.Location), "Resources");
  }
}