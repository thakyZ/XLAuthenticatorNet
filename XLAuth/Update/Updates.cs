using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using CheapLoc;
using Newtonsoft.Json;
using NuGet.Versioning;
using Velopack;
using Velopack.Locators;
using Velopack.Sources;
using XLAuth.Config;
using XLAuth.Models.Events;
using XLAuth.Models.Exceptions;
using XLAuth.Windows;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace XLAuth.Update;

/// <summary>
/// The updates class
/// </summary>
internal sealed partial class Updates : IDisposable {
#region Events
  internal event EventHandler<BooleanEventArgs>? UpdateCheckFinished;
#endregion Events

#region Fields
  /// <summary>
  /// The update manager
  /// </summary>
  private UpdateManager? _updateManager;
#endregion Fields

#region Constants
#if DEV_SERVER
  /// <summary>
  /// The lease file url
  /// </summary>
  private const string _LEASE_META_URL = "http://localhost:5025/xl-auth/GetLease";

  /// <summary>
  /// The lease meta url
  /// </summary>
  private const string _LEASE_FILE_URL = "http://localhost:5025/xl-auth/GetFile";
#else
  /// <summary>
  /// The lease meta url
  /// </summary>
  private const string _LEASE_META_URL = "https://api.nekogaming.xyz/ffxiv/xl-auth/GetLease";

  /// <summary>
  /// The lease file url
  /// </summary>
  private const string _LEASE_FILE_URL = "https://api.nekogaming.xyz/ffxiv/xl-auth/GetFile";
#endif

  /// <summary>
  /// The release track identifier
  /// </summary>
  private const string _TRACK_RELEASE = "Release";

  /// <summary>
  /// The pre-release track identifier
  /// </summary>
  private const string _TRACK_PRERELEASE = "PreRelease";

  /// <summary>
  /// Defines the fake url prefix
  /// </summary>
  private const string _FAKE_URL_PREFIX = "https://example.com/";
#endregion Constants

#region Properties
  /// <summary>
  /// Gets or sets the value of the update lease
  /// </summary>
  private static Lease? UpdateLease { get; set; }

  /// <summary>
  /// Gets or sets the Logger to use.
  /// </summary>
  private ILogger Logger { get; set; }

  /// <summary>
  /// Gets or sets the value of the <see cref="VelopackLocator" />.
  /// </summary>
  internal VelopackLocator Locator { get; }

  /*
   * Disabled due to being obsolete, the library handles this automatically.
  /// <summary>
  /// Gets or sets the value of the <see cref="Shortcuts" />.
  /// </summary>
  internal Shortcuts Shortcuts { get; }
   */
#endregion Properties

#region Constructors
  public Updates() {
    this.Logger = Support.Logger.GetMicrosoftContext();

    if (OperatingSystem.IsWindows()) {
      this.Locator = new WindowsVelopackLocator(this.Logger);
    } else if (OperatingSystem.IsLinux()) {
      this.Locator = new LinuxVelopackLocator(this.Logger);
    } else if (OperatingSystem.IsMacOS()) {
      this.Locator = new OsxVelopackLocator(this.Logger);
    } else {
      throw new UnsupportedOSException();
    }
    /*
     * Disabled due to being obsolete, the library handles this automatically.
    this.Shortcuts = new Shortcuts(this.Logger, this.Locator);
     */
  }
#endregion Constructors

#region Tasks
  /// <summary>
  /// Leases the update manager using the specified pre-release
  /// </summary>
  /// <param name="prerelease">A <see langword="bool" /> determining if a pre-release should be downloaded.</param>
  /// <exception cref="LeaseAcquisitionException">Thrown if the lease was failed to be retrieved.</exception>
  /// <returns>A task containing the update result</returns>
  private async Task<UpdateResult> LeaseUpdateManagerAsync(bool prerelease) {
    App.HttpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("XIVLauncher", Util.GetGitHash()));
    App.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-Track",       prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE);
    App.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-LV",          "0");
    App.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-HaveVersion", Util.GetAssemblyVersion());
    App.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-FirstStart",  App.Settings.VersionUpgradeLevel == 0 ? "yes" : "no");
    App.HttpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-HaveWine",    EnvironmentSettings.IsWine ? "yes" : "no");
    using HttpResponseMessage response = await App.HttpClient.GetAsync(_LEASE_META_URL).ConfigureAwait(false);
    response.EnsureSuccessStatusCode();
    if (response.Headers.TryGetValues("X-XL-Canary", out IEnumerable<string>? values) && values.FirstOrDefault()?.Equals("yes", StringComparison.OrdinalIgnoreCase) == true) {
      Support.Logger.Information("Updates: Received canary track lease!");
    }

    Lease? leaseData = JsonConvert.DeserializeObject<Lease>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
    if (leaseData?.Success != true) {
      throw new LeaseAcquisitionException(leaseData?.Message ?? "Failed to acquire lease data");
    }

    var fakeDownloader = new FakeVelopackFileDownloader(leaseData, prerelease);
    var updateOptions = new UpdateOptions {
      AllowVersionDowngrade = true,
      ExplicitChannel = prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE,
    };

    var updateSource = new SimpleWebSource(_FAKE_URL_PREFIX, fakeDownloader);
    var manager     = new UpdateManager(updateSource, updateOptions, this.Logger, this.Locator);
    return new UpdateResult(manager, leaseData);
  }

  /// <summary>
  /// Gets the error news as a fallback.
  /// </summary>
  /// <returns>A task containing the <see cref="ErrorNewsData" /> or <see langword="null" />.</returns>
  private static async Task<ErrorNewsData?> GetErrorNewsAsync() {
    ErrorNewsData? newsData = null;

    try {
      const string NEWS_URL = "https://gist.githubusercontent.com/thakyZ/66c27c66ab3f87a36f9682655960df09/raw/neko-gaming-news.txt";
      App.HttpClient.Timeout = TimeSpan.FromSeconds(10);
      string text = await App.HttpClient.GetStringAsync(NEWS_URL).ConfigureAwait(false);
      newsData = JsonConvert.DeserializeObject<ErrorNewsData>(text);
    } catch (Exception newsEx) {
      Support.Logger.Error(newsEx, "Could not get error news");
    }

    if (!(DateTimeOffset.UtcNow.ToUnixTimeSeconds() > newsData?.ShowUntil)) {
      return newsData;
    }

    return default;
  }

  /// <summary>
  /// Runs the download pre-release
  /// </summary>
  /// <param name="downloadPrerelease">The download pre-release</param>
  /// <param name="changelogWindow">The changelog window</param>
  internal async Task RunAsync(bool downloadPrerelease, ChangelogWindow? changelogWindow) {
    // GitHub requires TLS 1.2, we need to hard code this for Windows 7
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    try {
      UpdateResult updateResult = await this.LeaseUpdateManagerAsync(downloadPrerelease).ConfigureAwait(false);
      UpdateLease = updateResult.Lease;
#if DEBUG
      // Log feature flags
      try {
        var flags = string.Join(", ", Enum.GetValues(typeof(LeaseFeatureFlags)).Cast<LeaseFeatureFlags>().Where(f => UpdateLease.Flags.HasFlag(f)).Select(f => f.ToString()));
        Support.Logger.Information("Feature flags: {Flags}", flags);
      } catch (Exception ex) {
        Support.Logger.Error(ex, "Could not log feature flags");
      }
#endif

      this._updateManager = updateResult.Manager;
      VelopackApp.Build()
        .WithBeforeUninstallFastCallback((SemanticVersion _) => Updates.HandleBeforeUninstall()).Run();
      UpdateInfo? newRelease = await this._updateManager.CheckForUpdatesAsync().ConfigureAwait(false);
      if (newRelease is not null) {
        if (changelogWindow is null) {
          Support.Logger.Error("changelogWindow was null");
          this._updateManager.ApplyUpdatesAndRestart(newRelease);
          return;
        }

        try {
          await changelogWindow.Dispatcher.InvokeAsync(() => {
            changelogWindow.UpdateVersion(newRelease.TargetFullRelease.Version.ToString());
            changelogWindow.Show();
            changelogWindow.Closed += OnChangeLogWindowClosedWrapper;
          });
#if !XL_NOAUTOUPDATE
          this.OnUpdateCheckFinished(result: false);
#endif
        } catch (Exception ex) {
          Support.Logger.Error(ex, "Could not show changelog window");
          this._updateManager.ApplyUpdatesAndRestart(newRelease);
        } finally {
          if (changelogWindow is not null) {
            changelogWindow.Closed -= OnChangeLogWindowClosedWrapper;
          }
        }

        void OnChangeLogWindowClosedWrapper(object? sender, EventArgs @event) {
          this.OnChangeLogWindowClosed(sender, @event, newRelease.TargetFullRelease);
        }
#if !XL_NOAUTOUPDATE
      } else {
        this.OnUpdateCheckFinished(result: false);
#endif
      }
    } catch (Exception ex) {
      Support.Logger.Error(ex, "Update failed");
      ErrorNewsData? newsData = await Updates.GetErrorNewsAsync().ConfigureAwait(false);
      if (newsData is not null && !string.IsNullOrEmpty(newsData.Message)) {
        CustomMessageBox.Show(newsData.Message, "XIVLauncher", MessageBoxButton.OK,
          newsData.IsError ? MessageBoxImage.Error : MessageBoxImage.Asterisk);
      }
      else {
        CustomMessageBox.Show(
          Loc.Localize("UpdateFailureError",
            "XIVLauncher failed to check for updates. This may be caused by internet connectivity issues. Wait a few minutes and try again.\nDisable your VPN, if you have one. You may also have to exclude XIVLauncher from your antivirus.\nIf this continues to fail after several minutes, please check out the FAQ."),
          "XIVLauncher", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      this.Dispose();
      Environment.Exit(1);
    }

    // Reset security protocol after updating
    ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
  }
  #endregion Tasks

  /// <summary>
  /// Checks if <see cref="Updates.UpdateLease" /> has a <see cref="LeaseFeatureFlags" /> using the specified flag.
  /// </summary>
  /// <param name="flag">The flag to check</param>
  /// <returns><see langword="true" /> if the <see cref="Updates.UpdateLease" /> has the flag; otherwise <see langword="false" /></returns>
  internal static bool HasFeatureFlag(LeaseFeatureFlags flag)
    => UpdateLease?.Flags.HasFlag(flag) == true;

  /// <summary>
  /// Wrapper for the method <see cref="VelopackApp.WithBeforeUninstallFastCallback" /> in type <see cref="VelopackApp" />.
  /// </summary>
  private static void HandleBeforeUninstall() {
    if (CustomMessageBox.Show(Loc.Localize("UninstallQuestion", "Sorry to see you go!\nDo you want to delete all of your saved settings, plugins and passwords?"), "XIVLauncher", MessageBoxButton.YesNo, MessageBoxImage.Question, showHelpLinks: false, showDiscordLink: false) != MessageBoxResult.Yes) {
      return;
    }

    try {
      foreach (TOTPAccount account in App.AccountManager.Accounts) {
        account.Token = null;
        App.AccountManager.RemoveAccount(account);
      }
    } catch (Exception ex) {
      Support.Logger.Error(ex, "Uninstall: Could not delete passwords");
    }

    try {
      // Let's just give this a shot, probably not going to work 100% but
      // there's not really much we can do about it right now
      Directory.Delete(Paths.RoamingPath, recursive: true);
    } catch (Exception ex) {
      Support.Logger.Error(ex, "Uninstall: Could not delete roaming directory");
    }
  }

#region Event Invokers
  /// <summary>
  /// Called when the changelog window is closed.
  /// </summary>
  /// <param name="_">The sender of this event.</param>
  /// <param name="__">The event arguments.</param>
  private void OnChangeLogWindowClosed(object? _, EventArgs __, VelopackAsset asset) {
    this._updateManager?.ApplyUpdatesAndRestart(asset);
  }

#if !XL_NOAUTOUPDATE
  /// <summary>
  /// Called when the <see cref="UpdateManager" /> finished checking for updates.
  /// </summary>
  /// <param name="result">The result of the update checker.</param>
  private void OnUpdateCheckFinished(bool result) {
    var eventArgs = new BooleanEventArgs(result);
    this.UpdateCheckFinished?.Invoke(this, eventArgs);
  }
#endif
#endregion Event Invokers

#region IDisposable Implementation
  /// <summary>
  /// Disposes of this instance
  /// </summary>
  public void Dispose() {
    // With Velopack the UpdateManager is not type of IDisposable.
    //this._updateManager?.Dispose();
  }
#endregion IDisposable Implementation
}
