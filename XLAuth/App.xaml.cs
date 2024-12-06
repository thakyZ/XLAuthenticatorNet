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
  internal static AuthSettingsV1 Settings { get; private set; } = null!;

  /// <summary>
  /// Gets the value of the account manager
  /// </summary>
  internal static AccountManager AccountManager { get; } = new();

  /// <summary>
  /// Gets or sets the value of the command line
  /// </summary>
  private static CmdLineOptions? CommandLine { get; set; }

  /// <summary>
  /// Gets or sets settings value indicated if no OTP key is pushed upon the app opening.
  /// </summary>
  internal static bool GlobalIsDisableAutoSendOTP { get; private set; } = false;

  /// <summary>
  /// Gets or sets the value of the account picker width
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
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
  /// Gets or sets the value of the use full exception handler
  /// </summary>
  private bool UseFullExceptionHandler { get; set; }

  public static ImageSource? Icon { get; } = ResourceHelpers.LoadAppIcon();

  /// <summary>
  /// Gets the value of the serializer settings
  /// </summary>
  internal static JsonSerializerSettings SerializerSettings => new() {
    Formatting = Formatting.Indented,
    Error = (object? _, ErrorEventArgs args) => {
      Logger.Error(args.ErrorContext.Error, "Error while serializing accounts in accounts file.");
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

    for (var index = 0; index < count; index++) {
      DependencyObject myChild = VisualTreeHelper.GetChild(parent, index);

      if (myChild is FrameworkElement element && element.Name?.Equals(controlName, StringComparison.OrdinalIgnoreCase) == true) {
        return element;
      }

      DependencyObject? findResult = App.FindChildByName(myChild, controlName);

      if (findResult is not null) {
        return findResult;
      }
    }

    return null;
  }

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
  private Style GetStyleInternal(string key) {
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
    return ((Current as App)?.GetStyleInternal(key)) ?? throw new Exception("Failed to get Style.");
  }

  /// <summary>
  /// Gets the brush impl using the specified key
  /// </summary>
  /// <param name="key">The key</param>
  /// <param name="type">The type</param>
  /// <exception cref="Exception">Could not find brush with key, \"{key}\".</exception>
  /// <returns>The brush</returns>
  private Brush GetBrushInternal(string key, Type? type = null) {
    if (this.TryFindResource(key) is Brush brush1) {
      return brush1;
    }

    if (this.Resources.FindName(key) is Brush brush2) {
      return brush2;
    }

    if (this.Resources.MergedDictionaries.Where(resourceDictionary => resourceDictionary.FindName(key) is not null).Select(resourceDictionary => resourceDictionary.FindName(key)).FirstOrDefault() is Brush brush3) {
      return brush3;
    }

    if (new DynamicResourceExtension(key).ProvideValue(serviceProvider: null) is Brush brush4) {
      return brush4;
    }

    if (type is not null && new ServiceProviders().GetService(type) is IServiceProvider provider && new StaticResourceExtension(key).ProvideValue(provider) is Brush brush5) {
      return brush5;
    }

#if DEBUG
    foreach (KeyValuePair<object, object?> brushKey in this.Resources.MergedDictionaries.SelectMany(resourceDictionary => resourceDictionary.ToList()).Where(keyValuePair => keyValuePair.Value is Brush)) {
      Logger.Debug("brushKey.Key:   {0}", brushKey.Key);
      Logger.Debug("brushKey.Value: {0}", brushKey.Value);
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
  [SuppressMessage("Maintainability", "AV1551:Method overload should call another overload")]
  internal static Brush GetBrush(string key, Type? type = null) {
    Brush? output = (Current as App)?.GetBrushInternal(key, type);

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
  private Brush GetMaterialDesignBrushInternal(string assembly, string color, bool isPrimary, string key) {
    var brushUrl = string.Format(CultureInfo.InvariantCulture, "/{0};component/Themes/{0}.{1}.{2}.xaml", assembly, color, isPrimary ? "Primary" : "Secondary");
    var uri = new Uri(brushUrl, UriKind.RelativeOrAbsolute);
    ResourceDictionary? output;

    if (this.Resources.MergedDictionaries.All((ResourceDictionary resourceDictionary) => resourceDictionary.Source != uri)) {
      output = new ResourceDictionary {
        Source = uri,
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
  internal static Brush GetMaterialDesignBrush(string assembly, string color, bool isPrimary, string key) {
    Brush? output = (Current as App)?.GetMaterialDesignBrushInternal(assembly, color, isPrimary, key);

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
    } catch (Exception exception) {
      Logger.Error(exception, "Failed to get theme.");

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
  /// <param name="updateOTP">The update OTP</param>
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
  /// <param name="updateOTP">The update OTP</param>
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
  /// <param name="finishUp">The event args if we should finish up and start the main window.</param>
  private void OnUpdateCheckFinished(object? server, BooleanEventArgs finishUp) {
    this.Dispatcher?.Invoke(() => {
      this.UseFullExceptionHandler = true;
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
  [SuppressMessage("Design", "MA0051:Method is too long", Justification = "Startup method always the heaviest.")]
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
      var logOutput = Path.Combine(Paths.RoamingPath, "output.log");
      LogInit.Setup(logOutput, Environment.GetCommandLineArgs());
#if !DEBUG
      AppDomain.CurrentDomain.UnhandledException += EarlyInitExceptionHandler;
      TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
#endif
      Logger.Information("========================================================");
      var assemblyVersion = Util.GetAssemblyVersion();
      var gitHash = Util.GetGitHash();
      Logger.Information("Starting a session(v{Version} - {Hash})", assemblyVersion, gitHash);
    } catch (Exception exception) {
      CustomMessageBox.Show($"Could not set up logging. Please report this error.\n\n{exception.Message}", "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    try {
      var helpWriter = new StringWriter();

      var parser = new Parser(config => {
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
      Settings = App.SetupSettingsImpl();
    } catch (Exception exception) {
      Logger.Error(exception, "Failed to setup settings.");
      var configPath = App.GetConfigPath("auth");
      File.Delete(configPath);
      Settings = App.SetupSettingsImpl();
    }

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
  /// Setup the settings impl
  /// </summary>
  /// <returns>The output</returns>
  private static AuthSettingsV1 SetupSettingsImpl() {
    var output = AuthSettingsV1.Load();
    Logger.LoggingLevelSwitch.MinimumLevel = output.LogLevel;

    return output;
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

      if (UseFullExceptionHandler) {
        CustomMessageBox.Builder.NewFrom((Exception)e.ExceptionObject, "Unhandled", CustomMessageBox.ExitOnCloseModes.ExitOnClose).WithAppendText("\n\nError during early initialization. Please report this error.\n\n" + e.ExceptionObject).Show();
      } else {
        MessageBox.Show("Error during early initialization. Please report this error.\n\n" + e.ExceptionObject, "XIVLauncher Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      Environment.Exit(-1);
    });
  }
#endif
}
