using System;
using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using Serilog;
using Serilog.Events;
using Config.Net;

using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Windows;
using XLAuthenticatorNet.Support;

namespace XLAuthenticatorNet {

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application {
    public static IAuthSettingsV1 Settings;

    /*
#if !NOAUTOUPDATE
    private UpdateLoadingDialog _updateWindow;
#endif
    */

    internal MainWindow _mainWindow = null;

    public App() {
      try {
        if (EnvironmentSettings.IsWine)
          RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
      } catch {
        // ignored
      }

      var release = $"xivlauncher-{Util.GetAssemblyVersion()}-{Util.GetGitHash()}";

      try {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Async(a => a.File(Path.Combine(Paths.RoamingPath, "output.log")))
            .WriteTo.Sink(SerilogEventSink.Instance)
#if DEBUG
            .WriteTo.Debug()
            .MinimumLevel.Verbose()
#else
            .MinimumLevel.Information()
#endif
            .CreateLogger();

#if !DEBUG
        AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
        TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
#endif
      } catch (Exception ex) {
        MessageBox.Show("Could not set up logging. Please report this error.\n\n" + ex.Message, "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      SerilogEventSink.Instance.LogLine += OnSerilogLogLine;

      try {
        SetupSettings();
      } catch (Exception e) {
        Log.Error(e, "Failed to setup settings.");
        File.Delete(GetConfigPath("launcher"));
        SetupSettings();
      }

#if !XL_NOAUTOUPDATE
      if (!EnvironmentSettings.IsDisableUpdates) {
        try {
          Log.Information("Starting update check...");

          //_updateWindow = new UpdateLoadingDialog();
          //_updateWindow.Show();

          var updateMgr = new Updates();
          updateMgr.OnUpdateCheckFinished += OnUpdateCheckFinished;

          updateMgr.Run(EnvironmentSettings.IsPreRelease);
        } catch (Exception ex) {
          MessageBox.Show(
              "XIVLauncher could not contact the update server. Please check your internet connection or try again.\n\n" + ex,
              "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
          Environment.Exit(0);
        }
      }
#endif
    }

    public static bool SetupDevice { get; set; } = false;

    private bool _useFullExceptionHandler = false;

    private void OnUpdateCheckFinished(bool finishUp) {
      Dispatcher.Invoke(() => {
        _useFullExceptionHandler = true;

#if !NOAUTOUPDATE
        //if (_updateWindow is not null) {
        //  _updateWindow.Hide
        //}
#endif

        if (!finishUp)
          return;

        _mainWindow = new MainWindow();
        _mainWindow.Initialize();
      });
    }

    private static void OnSerilogLogLine(object sender, (string Line, LogEventLevel Level, DateTimeOffset TimeStamp, Exception Exception) e) {
      if (e.Exception == null)
        return;
    }

    private void App_OnStartup(object sender, StartupEventArgs e) {
      Console.WriteLine("[INF] Loading Window...");
      if (EnvironmentSettings.IsDisableUpdates) {
        OnUpdateCheckFinished(true);
      }
#if NOAUTOUPDATE
      OnUpdateCheckFinished(null, null);
#endif
    }

    private void SetupSettings() {
      Settings = new ConfigurationBuilder<IAuthSettingsV1>()
          .UseCommandLineArgs()
          .UseJsonFile(GetConfigPath("auth"))
          .Build();
    }

    private static string GetConfigPath(string prefix) => Path.Combine(Paths.RoamingPath, $"{prefix}ConfigV1.json");
  }
}