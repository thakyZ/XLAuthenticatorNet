using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using CommandLine;
using Serilog;
using Serilog.Core;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Support;
using XLAuthenticatorNet.Windows;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;
#if DEBUG
using System.Collections.Generic;
using XLAuthenticatorNet.Extensions;
#else
#if !XL_NOAUTOUPDATE
using System.Windows.Threading;
using System.Threading.Tasks;
#endif

#endif

namespace XLAuthenticatorNet;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application {
  /// <summary>
  /// The repo url
  /// </summary>
  public const string REPO_URL = "https://github.com/thakyZ/XLAuthenticatorNET";
  /// <summary>
  /// Gets the value of the logging level switch
  /// </summary>
  private static LoggingLevelSwitch LoggingLevelSwitch => new LoggingLevelSwitch();
  /// <summary>
  /// Gets or sets the value of the settings
  /// </summary>
  internal static AuthSettingsV1 Settings { get; private set; } = null!;
  /// <summary>
  /// Gets the value of the account manager
  /// </summary>
  internal static AccountManager AccountManager { get; } = new AccountManager();
  /// <summary>
  /// Gets or sets the value of the command line
  /// </summary>
  private static CmdLineOptions? CommandLine { get; set; }

  /// <summary>
  /// Gets or sets a value indicated if no OTP key is pushed upon the app opening.
  /// </summary>
  internal static bool GlobalIsDisableAutoSendOTP { get; private set; } = false;

  /// <summary>
  /// Gets or sets the value of the account picker width
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static int? AccountPickerWidth { get; set; }

  /// <summary>
  /// The from argb
  /// </summary>
  private static readonly Brush _uaBrush = new LinearGradientBrush([
    new GradientStop(Color.FromArgb(0xFF, 0x00, 0x57, 0xB7), 0.5f),
    new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xd7, 0x00), 0.5f),
  ], 0.7f);

#if !XL_NOAUTOUPDATE
  /// <summary>
  /// Gets or sets the value of the update loading window
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
  private UpdateLoadingWindow? UpdateLoadingWindow { get; set; }
#endif

  /// <summary>
  /// Gets or sets the value of the use full exception handler
  /// </summary>
  private bool UseFullExceptionHandler { get; set; }

  public static ImageSource? AppIcon { get; } = LoadAppIcon();

  /// <summary>
  /// Gets the value of the serializer settings
  /// </summary>
  internal static JsonSerializerSettings SerializerSettings => new JsonSerializerSettings {
    Formatting = Formatting.Indented,
    Error = (object? _, ErrorEventArgs args) => {
      Log.Error(args.ErrorContext.Error, "Error while serializing accounts in accounts file.");
      args.ErrorContext.Handled = true;
    },
    Culture = CultureInfo.InvariantCulture,
    MaxDepth = 10,
    DefaultValueHandling = DefaultValueHandling.Populate,
    ConstructorHandling = ConstructorHandling.Default,
  };

  /// <summary>
  /// Finds the child by name using the specified parent
  /// </summary>
  /// <param name="parent">The parent</param>
  /// <param name="controlName">The control name</param>
  /// <returns>The dependency object</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static DependencyObject? FindChildByName(DependencyObject parent, string controlName) {
    int count = VisualTreeHelper.GetChildrenCount(parent);

    for (var i = 0; i < count; i++) {
      DependencyObject myChild = VisualTreeHelper.GetChild(parent, i);

      if (myChild is FrameworkElement element && element.Name?.Equals(controlName, StringComparison.OrdinalIgnoreCase) == true) {
        return element;
      }

      DependencyObject? findResult = FindChildByName(myChild, controlName);

      if (findResult is not null) {
        return findResult;
      }
    }

    return null;
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="App"/> class
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

  /// <summary>
  /// Gets the config path using the specified prefix
  /// </summary>
  /// <param name="prefix">The prefix</param>
  /// <returns>The string</returns>
  private static string GetConfigPath(string prefix) => Path.Combine(Paths.RoamingPath, $"{prefix}ConfigV1.json");

  /// <summary>
  /// Gets the style impl using the specified key
  /// </summary>
  /// <param name="key">The key</param>
  /// <exception cref="Exception">Could not find style with key, \"{key}\".</exception>
  /// <returns>The style</returns>
  private Style GetStyleImpl(string key) {
    if (this.FindResource(key) is Style style) {
      return style;
    }

    throw new Exception($"Could not find style with key, \"{key}\".");
  }

  /// <summary>
  /// Gets the style using the specified key
  /// </summary>
  /// <param name="key">The key</param>
  /// <exception cref="Exception">Failed to get Style.</exception>
  /// <returns>The output</returns>
  internal static Style GetStyle(string key) {
    Style? output = (Current as App)?.GetStyleImpl(key);

    if (output is null) {
      throw new Exception("Failed to get Style.");
    }

    return output;
  }

  /// <summary>
  /// Gets the brush impl using the specified key
  /// </summary>
  /// <param name="key">The key</param>
  /// <param name="type">The type</param>
  /// <exception cref="Exception">Could not find brush with key, \"{key}\".</exception>
  /// <returns>The brush</returns>
  private Brush GetBrushImpl(string key, Type? type = null) {
    if (this.TryFindResource(key) is Brush brush1) {
      return brush1;
    }

    if (this.Resources.FindName(key) is Brush brush2) {
      return brush2;
    }

    if (this.Resources.MergedDictionaries.Where(x => x.FindName(key) is not null).Select(x => x.FindName(key)).FirstOrDefault() is Brush brush3) {
      return brush3;
    }

    if (new DynamicResourceExtension(key).ProvideValue(null) is Brush brush4) {
      return brush4;
    }

    if (type is not null && new ServiceProviders().GetService(type) is IServiceProvider provider && new StaticResourceExtension(key).ProvideValue(provider) is Brush brush5) {
      return brush5;
    }

#if DEBUG
    foreach (KeyValuePair<object, object?> brushKey in this.Resources.MergedDictionaries.SelectMany(resourceDictionary => resourceDictionary.ToList()).Where(x => x.Value is Brush)) {
      Log.Debug("brushKey.Key:   {0}", brushKey.Key);
      Log.Debug("brushKey.Value: {0}", brushKey.Value);
    }
#endif
    throw new Exception($"Could not find brush with key, \"{key}\".");
  }

  /// <summary>
  /// Gets the brush using the specified key
  /// </summary>
  /// <param name="key">The key</param>
  /// <param name="type">The type</param>
  /// <exception cref="Exception">Failed to get Brush.</exception>
  /// <returns>The output</returns>
  internal static Brush GetBrush(string key, Type? type = null) {
    Brush? output = (Current as App)?.GetBrushImpl(key, type);

    if (output is not Brush) {
      throw new Exception("Failed to get Brush.");
    }

    return output;
  }

  /// <summary>
  /// Gets the brush impl using the specified assembly
  /// </summary>
  /// <param name="assembly">The assembly</param>
  /// <param name="color">The color</param>
  /// <param name="isPrimary">The is primary</param>
  /// <param name="key">The key</param>
  /// <exception cref="Exception">Could not find brush with key, \"{key}\".</exception>
  /// <exception cref="Exception">Could not find resource with uri, \"{uri}\".</exception>
  /// <returns>The brush</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  private Brush GetBrushImpl(string assembly, string color, bool isPrimary, string key) {
    var uri = new Uri(string.Format("/{0};component/Themes/{0}.{1}.{2}.xaml", assembly, color, isPrimary ? "Primary" : "Secondary"), UriKind.RelativeOrAbsolute);
    ResourceDictionary? output;

    if (this.Resources.MergedDictionaries.All((ResourceDictionary resourceDictionary) => resourceDictionary.Source != uri)) {
      output = new ResourceDictionary {
        Source = uri
      };

      this.Resources.MergedDictionaries.Add(output);
    } else {
      output = this.Resources.MergedDictionaries.FirstOrDefault((ResourceDictionary resourceDictionary) => resourceDictionary.Source == uri);
    }

    if (output is null) {
      throw new Exception($"Could not find resource with uri, \"{uri}\".");
    }

    if (output.FindName(key) is Brush brush) {
      return brush;
    }

    throw new Exception($"Could not find brush with key, \"{key}\".");
  }

  /// <summary>
  /// Gets the brush using the specified assembly
  /// </summary>
  /// <param name="assembly">The assembly</param>
  /// <param name="color">The color</param>
  /// <param name="isPrimary">The is primary</param>
  /// <param name="key">The key</param>
  /// <exception cref="Exception">Failed to get Brush.</exception>
  /// <returns>The output</returns>
  internal static Brush GetBrush(string assembly, string color, bool isPrimary, string key) {
    Brush? output = (Current as App)?.GetBrushImpl(assembly, color, isPrimary, key);

    if (output is not Brush) {
      throw new Exception("Failed to get Brush.");
    }

    return output;
  }

  /// <summary>
  /// Gets the theme impl
  /// </summary>
  /// <returns>The theme</returns>
  private Theme? GetThemeImpl() {
    try {
      return this.Resources.GetTheme();
    } catch (Exception ex) {
      Log.Error(ex, "Failed to get theme.");

      return null;
    }
  }

  /// <summary>
  /// Gets the theme
  /// </summary>
  /// <returns>The theme</returns>
  internal static Theme? GetTheme() {
    return (Current as App)?.GetThemeImpl();
  }

  /// <summary>
  /// Refreshes the data impl using the specified all
  /// </summary>
  /// <param name="all">The all</param>
  /// <param name="updatePopupContent">The update popup content</param>
  /// <param name="updateOTP">The update otp</param>
  private void RefreshDataImpl(bool all = false, bool updatePopupContent = false, bool updateOTP = false) {
    if (this.MainWindow is not MainWindow mainWindow) {
      return;
    }

    mainWindow.SettingsControl.RefreshData(updatePopupContent: updatePopupContent || all);
    mainWindow.MainContent.RefreshData(updateOTP: updateOTP || all);
  }

  /// <summary>
  /// Refreshes the data using the specified all
  /// </summary>
  /// <param name="all">The all</param>
  /// <param name="updatePopupContent">The update popup content</param>
  /// <param name="updateOTP">The update otp</param>
  internal static void RefreshData(bool all = false, bool updatePopupContent = false, bool updateOTP = false) {
    (Current as App)?.RefreshDataImpl(all, updatePopupContent, updateOTP);
  }

  /// <summary>
  /// Reloads the settings impl
  /// </summary>
  private void ReloadSettingsImpl() {
    if (this.MainWindow is MainWindow mainWindow) {
      mainWindow.SettingsControl.ReloadSettings();
    }
  }

  /// <summary>
  /// Reloads the settings
  /// </summary>
  internal static void ReloadSettings() {
    (Current as App)?.ReloadSettingsImpl();
  }

  /// <summary>
  /// Ons the update check finished using the specified finish up
  /// </summary>
  /// <param name="finishUp">The finish up</param>
  private void OnUpdateCheckFinished(bool finishUp) {
    Dispatcher?.Invoke(() => {
      this.UseFullExceptionHandler = true;
#if !XL_NOAUTOUPDATE
      this.UpdateLoadingWindow?.Hide();
#endif
      if (!finishUp) {
        return;
      }

      this.MainWindow = new MainWindow();
    });
  }

  /// <summary>
  /// Apps the on startup using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void App_OnStartup(object? sender, StartupEventArgs e) {
    // HW rendering commonly causes issues with material design, so we turn it off by default for now
    try {
      if (!EnvironmentSettings.IsHardwareRendered) {
        RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
      }
    } catch {
      // ignored
    }

    try {
      Log.Logger = new LoggerConfiguration()
       .WriteTo.Async(a => a.File(Path.Combine(Paths.RoamingPath, "output.log")))
#if DEBUG
       .WriteTo.Async(a => a.Debug()).MinimumLevel.Verbose()
#else
       .MinimumLevel.ControlledBy(App.LoggingLevelSwitch)
#endif
       .CreateLogger();
#if !DEBUG
      AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
#endif
      Log.Information("========================================================");
      Log.Information("Starting a session(v{Version} - {Hash})", Util.GetAssemblyVersion(), Util.GetGitHash());
    } catch (Exception ex) {
      CustomMessageBox.Show($"Could not set up logging. Please report this error.\n\n{ex.Message}", "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    try {
      var helpWriter = new StringWriter();

      var parser = new Parser(config => {
        config.HelpWriter = helpWriter;
        config.IgnoreUnknownArguments = true;
      });

      ParserResult<CmdLineOptions>? result = parser.ParseArguments<CmdLineOptions>(Environment.GetCommandLineArgs());

      if (result.Errors.Any()) {
        MessageBox.Show(helpWriter.ToString(), "Help");
      }

      CommandLine = result.Value ?? new CmdLineOptions();

      if (!string.IsNullOrEmpty(CommandLine.RoamingPath)) {
        Paths.OverrideRoamingPath(CommandLine.RoamingPath);
      }

      if (CommandLine.NoAutoSendOTP) {
        GlobalIsDisableAutoSendOTP = true;
      }

      if (CommandLine.DoGenerateLocalizables) {
        GenerateLocalizables();
      }
    } catch (Exception ex) {
      MessageBox.Show("Could not parse command line arguments. Please report this error.\n\n" + ex.Message, "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    try {
      Settings = SetupSettingsImpl();
    } catch (Exception ex) {
      Log.Error(ex, "Failed to setup settings.");
      File.Delete(GetConfigPath("auth"));
      Settings = SetupSettingsImpl();
    }

#if !LOC_FORCEFALLBACKS
    try {
      if (Settings.Language == null) {
        string currentUiLang = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
        Settings.Language = Settings.Language.GetLangFromTwoLetterIso(currentUiLang);
      }

      Log.Information("Trying to set up Loc for language code {0}", Settings.Language.GetLocalizationCode());

      if (!Settings.Language.IsDefault() && Util.GetFromResources($"XIVLauncher.Resources.Loc.xl.xl_{Settings.Language.GetLocalizationCode()}.json") is string loc) {
        Loc.Setup(loc);
      } else {
        Loc.SetupWithFallbacks();
      }
    } catch (Exception ex) {
      Log.Error(ex, "Could not get language information. Setting up fallbacks.");
      Loc.Setup("{}");
    }
#else
    // Force all fallbacks
    Loc.Setup("{}");
#endif
#if !XL_NOAUTOUPDATE
    if (EnvironmentSettings.IsDisableUpdates) {
      try {
        Log.Information("Starting update check...");
        this.UpdateLoadingWindow = new UpdateLoadingWindow();
        this.UpdateLoadingWindow.Show();
        var updateMgr = new Updates();
        updateMgr.OnUpdateCheckFinished += OnUpdateCheckFinished;
        ChangelogWindow? changelogWindow = null;

        try {
          changelogWindow = new ChangelogWindow(EnvironmentSettings.IsPreRelease);
        } catch (Exception ex) {
          Log.Error(ex, "Could not load changelog window");
        }

        Task.Run(() => updateMgr.Run(EnvironmentSettings.IsPreRelease, changelogWindow));
      } catch (Exception ex) {
        Log.Error(ex, "Could not dispatch update check");
        MessageBox.Show("XIVLauncher could not check for updates. Please check your internet connection or try again.\n\n" + ex, "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
        Environment.Exit(0);

        return;
      }
    }
#endif
    try {
      if (Settings.Language == Language.Russian) {
        var dict = new ResourceDictionary {
          {
            "PrimaryHueLightBrush", _uaBrush
          },
          //{"PrimaryHueLightForegroundBrush", uaBrush},
          {
            "PrimaryHueMidBrush", _uaBrush
          },
          //{"PrimaryHueMidForegroundBrush", uaBrush},
          {
            "PrimaryHueDarkBrush", _uaBrush
          },
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
      OnUpdateCheckFinished(true);

      // ReSharper disable once RedundantJumpStatement
      return;
    }
#endif
#if XL_NOAUTOUPDATE
    OnUpdateCheckFinished(true);
#endif
  }

  /// <summary>
  /// Setup the settings impl
  /// </summary>
  /// <returns>The output</returns>
  private static AuthSettingsV1 SetupSettingsImpl() {
    var output = AuthSettingsV1.Load();
    LoggingLevelSwitch.MinimumLevel = output.LogLevel;

    return output;
  }

  /// <summary>
  /// Apps the on exit using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void App_OnExit(object sender, ExitEventArgs e) {
    AccountManager.Save();
    Settings.Save();
  }

  /// <summary>
  /// Generates the localizables
  /// </summary>
  private static void GenerateLocalizables() {
    try {
      Loc.ExportLocalizable();
    } catch (Exception ex) {
      MessageBox.Show(ex.ToString());
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
    if (!e.Observed) EarlyInitExceptionHandler(sender, new UnhandledExceptionEventArgs(e.Exception, true));
  }
#endif
  /// <summary>
  /// Earlies the init exception handler using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void EarlyInitExceptionHandler(object? sender, UnhandledExceptionEventArgs e) {
    this.Dispatcher.Invoke(() => {
      Log.Error((Exception)e.ExceptionObject, "Unhandled exception");

      if (UseFullExceptionHandler) {
        CustomMessageBox.Builder.NewFrom((Exception)e.ExceptionObject, "Unhandled", CustomMessageBox.ExitOnCloseModes.ExitOnClose).WithAppendText("\n\nError during early initialization. Please report this error.\n\n" + e.ExceptionObject).Show();
      } else {
        MessageBox.Show("Error during early initialization. Please report this error.\n\n" + e.ExceptionObject, "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      Environment.Exit(-1);
    });
  }
#endif

  /// <summary>
  /// Loads the app's icon from embedded resources.
  /// </summary>
  /// <returns>The app's icon as an image source.</returns>
  private static ImageSource? LoadAppIcon() {
    var       assembly = Assembly.GetExecutingAssembly();
    using var stream   = assembly.GetManifestResourceStream("xlauth_icon");
    if (stream is null) return null;
    var decoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
    return decoder.Frames[0];
  }

  /// <summary>
  /// The cmd line options class
  /// </summary>
  public class CmdLineOptions {
    /// <summary>
    /// Gets or sets the value of the roaming path
    /// </summary>
    [Option("roamingPath", Required = false, HelpText = "Path to a folder to override the roaming path for XL with.")]
    public string? RoamingPath { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the no auto send otp
    /// </summary>
    [Option("noAutoSendOTP", Required = false, HelpText = "Disable auto send OTP on start-up.")]
    public bool NoAutoSendOTP { get; set; }

    /// <summary>
    /// Gets or sets the value of the do generate localizables
    /// </summary>
    [Option("gen-localizable", Required = false, HelpText = "Generate localizable files.")]
    public bool DoGenerateLocalizables { get; set; }

    /// <summary>
    /// Gets or sets the value of the do generate integrity
    /// </summary>
    [Option("gen-integrity", Required = false, HelpText = "Generate integrity files. Provide a game path.")]
    public string? DoGenerateIntegrity { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the account name
    /// </summary>
    [Option("account", Required = false, HelpText = "Account name to use.")]
    public string? AccountName { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the language
    /// </summary>
    [Option("lang", Required = false, HelpText = "Language to use.")]
    public Language? Language { get; set; }

    // We don't care about these, just need it so that the parser doesn't error
    /// <summary>
    /// Gets or sets the value of the squirrel updated
    /// </summary>
    [Option("squirrel-updated", Hidden = true)]
    public string? SquirrelUpdated { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel install
    /// </summary>
    [Option("squirrel-install", Hidden = true)]
    public string? SquirrelInstall { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel obsolete
    /// </summary>
    [Option("squirrel-obsolete", Hidden = true)]
    public string? SquirrelObsolete { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel uninstall
    /// </summary>
    [Option("squirrel-uninstall", Hidden = true)]
    public string? SquirrelUninstall { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel first run
    /// </summary>
    [Option("squirrel-first-run", Hidden = true)]
    public bool SquirrelFirstRun { get; set; }
  }
}