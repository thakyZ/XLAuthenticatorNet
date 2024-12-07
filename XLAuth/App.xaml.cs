using System.IO;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using CommandLine;
using XLAuth.Config;
using XLAuth.Extensions;
using XLAuth.Windows;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
using XLAuth.Models.Events;
using System.Net.Http;
using System.Net.NetworkInformation;
using Microsoft.Extensions.DependencyInjection;

#if !XL_NOAUTOUPDATE
using XLAuth.Update;
#endif

namespace XLAuth;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  /// <summary>
  /// Gets or sets the value of the settings
  /// </summary>
  internal static AuthSettings Settings { get; private set; } = null!;

  /// <summary>
  /// Gets the value of the account manager
  /// </summary>
  internal static AccountManager AccountManager { get; } = new();

  /// <summary>
  /// Gets or sets the value of the command line
  /// </summary>
  private static CmdLineOptions? CommandLine { get; set; }

  /// <summary>
  /// Gets a value indicated if no OTP key is pushed upon the app opening.
  /// </summary>
  internal static bool GlobalIsDisableAutoSendOTP { get; private set; } = false;

  /// <summary>
  /// Gets the instance of an <see cref="HttpClient" /> for the whole app to use.
  /// </summary>
  internal static HttpClient HttpClient { get; } = new();

  /// <summary>
  /// Gets the instance of an <see cref="Ping" /> for the whole app to use.
  /// </summary>
  internal static Ping Ping { get; } = new();

  /// <summary>
  /// Gets or sets the value of the account picker width
  /// </summary>
  internal static int? AccountPickerWidth { get; set; }

  /// <summary>
  /// GHets a brush that is for representing the clear victims of the Ukraine war.
  /// </summary>
  private static readonly Brush _uaBrush = new LinearGradientBrush([
    new GradientStop(Color.FromArgb(0xFF, 0x00, 0x57, 0xB7), 0.5f),
    new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xd7, 0x00), 0.5f),
  ], 0.7f);

#if !XL_NOAUTOUPDATE
  /// <summary>
  /// Gets or sets the value of the update loading window
  /// </summary>
  private UpdateLoadingWindow? UpdateLoadingWindow { get; set; }
#endif

  /// <summary>
  /// Gets or sets a <see langword="bool" /> which determines whether the use the full exception handler.
  /// </summary>
  internal static bool UseFullExceptionHandler { get; set; }

  /// <summary>
  /// Gets the instance of the <see cref="Application" /> icon.
  /// </summary>
  public static ImageSource? Icon { get; } = ResourceHelpers.LoadAppIcon();

  /// <summary>
  /// Initializes settings new instance of the <see cref="App"/> class
  /// </summary>
  internal App() {
#if !DEBUG
    try {
      AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
    } catch {
      // ignored
    }
#endif
  }

  /// <inheritdoc cref="XLAuth.Models.Abstracts.IReloadableControl.RefreshData(RefreshPart)"/>
  internal static void RefreshData(RefreshPart part) {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    if (app.MainWindow is not MainWindow mainWindow) {
      return;
    }

    mainWindow.SettingsControl.RefreshData(part);
    mainWindow.MainContent.RefreshData(part);
  }

  /// <summary>
  /// Reloads the settings
  /// </summary>
  internal static void ReloadSettings() {
    if (Application.Current is not App app) {
      throw new Exception("Application.Current is not typeof App.");
    }

    if (app.MainWindow is MainWindow mainWindow) {
      mainWindow.SettingsControl.ReloadSettings();
    }
  }

  /// <summary>
  /// Ons the update check finished using the specified finish up
  /// </summary>
  /// <param name="finishUp">The event args if we should finish up and start the main window.</param>
  private void OnUpdateCheckFinished(object? server, BooleanEventArgs finishUp) {
    this.Dispatcher?.Invoke(() => {
      App.UseFullExceptionHandler = true;
#if !XL_NOAUTOUPDATE
      this.UpdateLoadingWindow?.Hide();
#endif
      if (!finishUp.Value) {
        return;
      }

      this.MainWindow = new MainWindow();
    });
  }

  /// <summary>
  /// Apps the on startup using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="event">The </param>
  private void OnStartup(object? sender, StartupEventArgs @event) {
    // HW rendering commonly causes issues with material design, so we turn it off by default for now
    try {
      if (!EnvironmentSettings.IsHardwareRendered) {
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
      }
    } catch {
      // ignored
    }

    try {
      LogInit.Setup(Path.Combine(Paths.RoamingPath, "output.log"), Environment.GetCommandLineArgs());
#if !DEBUG
      AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
#endif
      Logger.Information("========================================================");
      Logger.Information("Starting a session(v{Version} - {Hash})", Util.GetAssemblyVersion(), Util.GetGitHash());
    } catch (Exception exception) {
      CustomMessageBox.Show($"Could not set up logging. Please report this error.\n\n{exception.Message}", "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    try {
      var helpWriter = new StringWriter();

      using var parser = new Parser(config => {
        config.HelpWriter = helpWriter;
        config.IgnoreUnknownArguments = true;
      });

      var commandLineArgs = Environment.GetCommandLineArgs();
      ParserResult<CmdLineOptions>? result = parser.ParseArguments<CmdLineOptions>(commandLineArgs);

      if (result.Errors.Any()) {
        var helpText = helpWriter.ToString();
        MessageBox.Show(helpText, "Help");
      }

      CommandLine = result.Value ?? new CmdLineOptions();

      if (!string.IsNullOrEmpty(CommandLine.RoamingPath)) {
        Paths.OverrideRoamingPath(CommandLine.RoamingPath);
      }

      if (CommandLine.NoAutoSendOTP) {
        GlobalIsDisableAutoSendOTP = true;
      }

      if (CommandLine.DoGenerateLocalizables) {
        App.GenerateLocalizables();
      }
    } catch (Exception exception) {
      MessageBox.Show("Could not parse command line arguments. Please report this error.\n\n" + exception.Message, "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    try {
      Settings = AuthSettings.Load();
    } catch (Exception exception) {
      Logger.Error(exception, "Failed to setup settings.");
      var configPath = Util.GetConfigPath();
      File.Delete(configPath);
      Settings = AuthSettings.Load();
    }
    Logger.LoggingLevelSwitch.MinimumLevel = Settings.LogLevel;

#if !LOC_FORCEFALLBACKS
    try {
      if (Settings.Language == null) {
        string currentUiLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Settings.Language = Settings.Language.GetLangFromTwoLetterIso(currentUiLang);
      }

      var localizationCode = Settings.Language.GetLocalizationCode();
      Logger.Information("Trying to set up Loc for language code {0}", localizationCode);

      if (!Settings.Language.IsDefault() && ResourceHelpers.GetFromResources<string>($"XIVLauncher.Resources.Loc.xl.xl_{Settings.Language.GetLocalizationCode()}.json") is string loc) {
        Loc.Setup(loc);
      } else {
        Loc.SetupWithFallbacks();
      }
    } catch (Exception exception) {
      Logger.Error(exception, "Could not get language information. Setting up fallbacks.");
      Loc.Setup("{}");
    }
#else
    // Force all fallbacks
    Loc.Setup("{}");
#endif
#if !XL_NOAUTOUPDATE
    if (EnvironmentSettings.IsDisableUpdates) {
      try {
        Logger.Information("Starting update check...");
        this.UpdateLoadingWindow = new UpdateLoadingWindow();
        this.UpdateLoadingWindow.Show();
        var updateMgr = new Updates();
        updateMgr.UpdateCheckFinished += this.OnUpdateCheckFinished;
        ChangelogWindow? changelogWindow = null;

        try {
          changelogWindow = new ChangelogWindow(EnvironmentSettings.IsPreRelease);
        } catch (Exception ex) {
          Logger.Error(ex, "Could not load changelog window");
        }

        _ = Task.Run(async () => await updateMgr.RunAsync(EnvironmentSettings.IsPreRelease, changelogWindow));
      } catch (Exception ex) {
        Logger.Error(ex, "Could not dispatch update check");
        MessageBox.Show("XIVLauncher could not check for updates. Please check your internet connection or try again.\n\n" + ex, "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(0);

        return;
      }
    }
#endif
    try {
      if (Settings.Language == Language.Russian) {
        var dict = new ResourceDictionary {
          { "PrimaryHueLightBrush", _uaBrush },
          //{"PrimaryHueLightForegroundBrush", uaBrush},
          { "PrimaryHueMidBrush", _uaBrush },
          //{"PrimaryHueMidForegroundBrush", uaBrush},
          { "PrimaryHueDarkBrush", _uaBrush },
          //{"PrimaryHueDarkForegroundBrush", uaBrush},
        };

        this.Resources.MergedDictionaries.Add(dict);
      }
    } catch {
      // ignored
    }

#if !XL_NOAUTOUPDATE
    // ReSharper disable once InvertIf
    if (EnvironmentSettings.IsDisableUpdates) {
      OnUpdateCheckFinished(this, new BooleanEventArgs(value: true));

      // ReSharper disable once RedundantJumpStatement
      return;
    }
#endif
#if XL_NOAUTOUPDATE
    this.OnUpdateCheckFinished(this, new BooleanEventArgs(value: true));
#endif
  }

  /// <summary>
  /// Triggered when the <see cref="App" /> exits.
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="event">The event</param>
  private void OnExit(object sender, ExitEventArgs @event) {
    AccountManager.Save();
    Settings.Save();
  }

  /// <summary>
  /// Generates the localizables
  /// </summary>
  private static void GenerateLocalizables() {
    try {
      Loc.ExportLocalizable();
    } catch (Exception exception) {
      var message = exception.ToFullyQualifiedString();
      MessageBox.Show(message);
    }

    Environment.Exit(0);
  }

#if !DEBUG
#if !XL_NOAUTOUPDATE
  /// <summary>
  /// Tasks the scheduler on unobserved task exception using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void TaskSchedulerOnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e) {
    if (!e.Observed) EarlyInitExceptionHandler(sender, new UnhandledExceptionEventArgs(e.Exception, isTerminating: true));
  }
#endif
  /// <summary>
  /// Earlies the init exception handler using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void EarlyInitExceptionHandler(object? sender, UnhandledExceptionEventArgs e) {
    this.Dispatcher.Invoke(() => {
      Logger.Error((Exception)e.ExceptionObject, "Unhandled exception");

      if (App.UseFullExceptionHandler) {
        CustomMessageBox.Builder.NewFrom((Exception)e.ExceptionObject, "Unhandled", CustomMessageBox.ExitOnCloseModes.ExitOnClose).WithAppendText("\n\nError during early initialization. Please report this error.\n\n" + e.ExceptionObject).Show();
      } else {
        MessageBox.Show("Error during early initialization. Please report this error.\n\n" + e.ExceptionObject, "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      Environment.Exit(-1);
    });
  }
#endif
}
