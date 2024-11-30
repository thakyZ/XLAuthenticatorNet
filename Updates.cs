using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;
using CheapLoc;
using Newtonsoft.Json;
using Serilog;
using Squirrel;
using Squirrel.Sources;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet;

/// <summary>
/// The updates class
/// </summary>
internal class Updates {
  internal event Action<bool>? OnUpdateCheckFinished;
  /// <summary>
  /// The update manager
  /// </summary>
  private UpdateManager? _updateManager;
#if DEV_SERVER
  private const string _LEASE_META_URL = "http://localhost:5025/xl-auth/GetLease";
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
  /// The track release
  /// </summary>
  private const string _TRACK_RELEASE = "Release";
  /// <summary>
  /// The track prerelease
  /// </summary>
  private const string _TRACK_PRERELEASE = "Prerelease";
  /// <summary>
  /// Gets or sets the value of the update lease
  /// </summary>
  private static Lease? UpdateLease { get; set; }
  /// <summary>
  /// The error news data class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
  internal class ErrorNewsData {
    /// <summary>
    /// Gets or sets the value of the show until
    /// </summary>
    [JsonPropertyName("until")] internal uint ShowUntil { get; set; }
    /// <summary>
    /// Gets or sets the value of the message
    /// </summary>
    [JsonPropertyName("message")] internal required string Message { get; set; }
    /// <summary>
    /// Gets or sets the value of the is error
    /// </summary>
    [JsonPropertyName("isError")] internal bool IsError { get; set; }
  }
  /// <summary>
  /// The lease class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global"),
   SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal class Lease {
    /// <summary>
    /// Gets or sets the value of the success
    /// </summary>
    internal bool Success { get; set; }
    /// <summary>
    /// Gets or sets the value of the message
    /// </summary>
    internal string? Message { get; set; }
    /// <summary>
    /// Gets or sets the value of the cut-off boot ver
    /// </summary>
    internal string? CutOffBootVer { get; set; }
    /// <summary>
    /// Gets or sets the value of the frontier url
    /// </summary>
    internal required string FrontierUrl { get; set; }
    /// <summary>
    /// Gets or sets the value of the flags
    /// </summary>
    internal LeaseFeatureFlags Flags { get; set; }
    /// <summary>
    /// Gets or sets the value of the releases list
    /// </summary>
    internal required string ReleasesList { get; set; }
    /// <summary>
    /// Gets or sets the value of the valid until
    /// </summary>
    internal DateTime? ValidUntil { get; set; }
  }
  /// <summary>
  /// The fake url prefix
  /// </summary>
  private const string _FAKE_URL_PREFIX = "https://example.com/";

  /// <summary>
  /// The lease feature flags enum
  /// </summary>
  [Flags]
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal enum LeaseFeatureFlags {
    /// <summary>
    /// The none lease feature flags
    /// </summary>
    None = 0,
    /// <summary>
    /// The global disable xl auth lease feature flags
    /// </summary>
    GlobalDisableXLAuth = 1,
    /// <summary>
    /// The force proxy xl auth and assets lease feature flags
    /// </summary>
    ForceProxyXLAuthAndAssets = 1 << 1,
  }

  /// <summary>
  /// The fake squirrel file downloader class
  /// </summary>
  /// <seealso cref="IFileDownloader"/>
  private class FakeSquirrelFileDownloader : IFileDownloader {
    /// <summary>
    /// The lease
    /// </summary>
    private readonly Lease _lease;
    /// <summary>
    /// The http client
    /// </summary>
    private readonly HttpClient _client = new HttpClient();

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeSquirrelFileDownloader"/> class
    /// </summary>
    /// <param name="lease">The lease</param>
    /// <param name="prerelease">The prerelease</param>
    internal FakeSquirrelFileDownloader(Lease lease, bool prerelease) {
      this._lease = lease;
      if (!_client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-Track",
            prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE)) {
        Log.Warning("Failed to add header \"X-XL-Track\" with value {Track} to the HTTP Client's default headers.",
          prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE);
      }
    }

    /// <summary>
    /// Downloads the file using the specified url
    /// </summary>
    /// <param name="url">The url</param>
    /// <param name="targetFile">The target file</param>
    /// <param name="progress">The progress</param>
    /// <param name="authorization">The authorization</param>
    /// <param name="accept">To auto </param>
    public async Task DownloadFile(string url, string targetFile, Action<int> progress, string? authorization = null, string? accept = null) {
      Log.Verbose("FakeSquirrel: DownloadFile from {Url} to {Target}", url, targetFile);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      using HttpResponseMessage response = await this._client.GetAsync($"{_LEASE_FILE_URL}/{fileNeeded}", HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      await using Stream contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
      await using Stream fileStream = File.Open(targetFile, FileMode.Create);
      await contentStream.CopyToAsync(fileStream).ConfigureAwait(false);
      fileStream.Close();
      Log.Verbose("FakeSquirrel: OK, downloaded {NumBytes}b for {File}", response.Content.Headers.ContentLength, fileNeeded);
    }

    /// <summary>
    /// Downloads the bytes using the specified url
    /// </summary>
    /// <param name="url">The url</param>
    /// <param name="authorization">The authorization</param>
    /// <param name="accept">The accept</param>
    /// <exception cref="ArgumentException">DownloadUrl called for unknown file: {url}</exception>
    /// <returns>A task containing the byte array</returns>
    public Task<byte[]> DownloadBytes(string url, string? authorization = null, string? accept = null) {
      Log.Verbose("FakeSquirrel: DownloadUrl from {Url}", url);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      if (fileNeeded.StartsWith("RELEASES", StringComparison.Ordinal)) {
        return Task.FromResult(Encoding.UTF8.GetBytes(this._lease.ReleasesList));
      }
      throw new ArgumentException($"DownloadUrl called for unknown file: {url}");
    }

    /// <summary>
    /// Downloads the string using the specified url
    /// </summary>
    /// <param name="url">The url</param>
    /// <param name="authorization">The authorization</param>
    /// <param name="accept">The accept</param>
    /// <exception cref="ArgumentException">DownloadUrl called for unknown file: {url}</exception>
    /// <returns>A task containing the string</returns>
    public Task<string> DownloadString(string url, string? authorization = null, string? accept = null) {
      Log.Verbose("FakeSquirrel: DownloadUrl from {Url}", url);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      if (fileNeeded.StartsWith("RELEASES", StringComparison.Ordinal)) {
        return Task.FromResult(this._lease.ReleasesList);
      }
      throw new ArgumentException($"DownloadUrl called for unknown file: {url}");
    }
  }

  /// <summary>
  /// The lease acquisition exception class
  /// </summary>
  /// <seealso cref="Exception"/>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public sealed class LeaseAcquisitionException : Exception {
    /// <summary>
    /// Initializes a new instance of the <see cref="LeaseAcquisitionException"/> class
    /// </summary>
    /// <param name="message">The message</param>
    internal LeaseAcquisitionException(string message) : base($"Couldn't acquire lease: {message}") { }
  }

  /// <summary>
  /// The update result class
  /// </summary>
  private sealed class UpdateResult {
    /// <summary>
    /// Gets or sets the value of the manager
    /// </summary>
    internal UpdateManager Manager { get; private set; }
    /// <summary>
    /// Gets or sets the value of the lease
    /// </summary>
    internal Lease Lease { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateResult"/> class
    /// </summary>
    /// <param name="manager">The manager</param>
    /// <param name="lease">The lease</param>
    internal UpdateResult(UpdateManager manager, Lease lease) {
      this.Manager = manager;
      this.Lease   = lease;
    }
  }

  /// <summary>
  /// Leases the update manager using the specified prerelease
  /// </summary>
  /// <param name="prerelease">The prerelease</param>
  /// <exception cref="LeaseAcquisitionException"></exception>
  /// <returns>A task containing the update result</returns>
  private static async Task<UpdateResult> LeaseUpdateManager(bool prerelease) {
    using var client = new HttpClient();
    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("XIVLauncher", Util.GetGitHash()));
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-Track", prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE);
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-LV", "0");
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-HaveVersion", Util.GetAssemblyVersion());
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-FirstStart", App.Settings.VersionUpgradeLevel == 0 ? "yes" : "no");
    client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-HaveWine", EnvironmentSettings.IsWine ? "yes" : "no");
    HttpResponseMessage response = await client.GetAsync(_LEASE_META_URL).ConfigureAwait(false);
    response.EnsureSuccessStatusCode();
    if (response.Headers.TryGetValues("X-XL-Canary", out IEnumerable<string>? values) && values.FirstOrDefault() == "yes") {
      Log.Information("Updates: Received canary track lease!");
    }

    Lease? leaseData = JsonConvert.DeserializeObject<Lease>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));
    if (leaseData?.Success != true) {
      throw new LeaseAcquisitionException(leaseData?.Message ?? "Failed to acquire lease data");
    }

    var fakeDownloader = new FakeSquirrelFileDownloader(leaseData, prerelease);
    var manager = new UpdateManager(_FAKE_URL_PREFIX, "XIVLauncher", null, fakeDownloader);
    return new UpdateResult(manager, leaseData);
  }

  /// <summary>
  /// Gets the error news
  /// </summary>
  /// <returns>A task containing the error news data</returns>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Local")]
  private static async Task<ErrorNewsData?> GetErrorNews() {
    ErrorNewsData? newsData = null;
    try {
      const string NEWS_URL = "https://gist.githubusercontent.com/thakyZ/66c27c66ab3f87a36f9682655960df09/raw/neko-gaming-news.txt";
      using var client = new HttpClient();
      client.Timeout = TimeSpan.FromSeconds(10);
      string text = await client.GetStringAsync(NEWS_URL).ConfigureAwait(false);
      newsData = JsonConvert.DeserializeObject<ErrorNewsData>(text);
    } catch (Exception newsEx) {
      Log.Error(newsEx, "Could not get error news");
    }

    return DateTimeOffset.UtcNow.ToUnixTimeSeconds() > newsData?.ShowUntil ? null : newsData;
  }

  /// <summary>
  /// Haves the feature flag using the specified flag
  /// </summary>
  /// <param name="flag">The flag</param>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static bool HaveFeatureFlag(LeaseFeatureFlags flag)
    => UpdateLease is not null && UpdateLease.Flags.HasFlag(flag);

  /// <summary>
  /// Runs the download prerelease
  /// </summary>
  /// <param name="downloadPrerelease">The download prerelease</param>
  /// <param name="changelogWindow">The changelog window</param>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal async Task Run(bool downloadPrerelease, ChangelogWindow? changelogWindow) {
    // GitHub requires TLS 1.2, we need to hardcode this for Windows 7
    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    try {
      UpdateResult updateResult = await LeaseUpdateManager(downloadPrerelease).ConfigureAwait(false);
      UpdateLease = updateResult.Lease;
      // Log feature flags
      try {
        var flags = string.Join(", ", Enum.GetValues(typeof(LeaseFeatureFlags)).Cast<LeaseFeatureFlags>().Where(f => UpdateLease.Flags.HasFlag(f)).Select(f => f.ToString()));
        Log.Information("Feature flags: {Flags}", flags);
      }
      catch (Exception ex) {
        Log.Error(ex, "Could not log feature flags");
      }

      this._updateManager = updateResult.Manager;
      SquirrelAwareApp.HandleEvents(onInitialInstall: (_, _) => this._updateManager.CreateShortcutForThisExe(),
        onAppUpdate: (_, _) => this._updateManager.CreateShortcutForThisExe(), onAppUninstall: (_, _) => {
          this._updateManager.RemoveShortcutForThisExe();
          if (CustomMessageBox.Show(Loc.Localize("UninstallQuestion", "Sorry to see you go!\nDo you want to delete all of your saved settings, plugins and passwords?"), "XIVLauncher", MessageBoxButton.YesNo, MessageBoxImage.Question, false, false) != MessageBoxResult.Yes)
            return;
          try {
            foreach (TotpAccount account in App.AccountManager.Accounts) {
              account.Token = null;
              App.AccountManager.RemoveAccount(account);
            }
          } catch (Exception ex) {
            Log.Error(ex, "Uninstall: Could not delete passwords");
          }

          try {
            // Let's just give this a shot, probably not going to work 100% but
            // there's not really much we can do about it right now
            Directory.Delete(Paths.RoamingPath, true);
          }
          catch (Exception ex) {
            Log.Error(ex, "Uninstall: Could not delete roaming directory");
          }
        });
      await _updateManager.CheckForUpdate().ConfigureAwait(false);
      ReleaseEntry? newRelease = await this._updateManager.UpdateApp().ConfigureAwait(false);
      if (newRelease is not null) {
        if (changelogWindow is null) {
          Log.Error("changelogWindow was null");
          UpdateManager.RestartApp();
          return;
        }

        try {
          changelogWindow.Dispatcher.Invoke(() => {
            changelogWindow.UpdateVersion(newRelease.Version.ToString());
            changelogWindow.Show();
            changelogWindow.Closed += (_, _) => { UpdateManager.RestartApp(); };
          });
#if !XL_NOAUTOUPDATE
          OnUpdateCheckFinished?.Invoke(false);
#endif
        }
        catch (Exception ex) {
          Log.Error(ex, "Could not show changelog window");
          UpdateManager.RestartApp();
        }
#if !XL_NOAUTOUPDATE
      } else {
        OnUpdateCheckFinished?.Invoke(true);
#endif
      }
    }
    catch (Exception ex) {
      Log.Error(ex, "Update failed");
      ErrorNewsData? newsData = await GetErrorNews().ConfigureAwait(false);
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

  /// <summary>
  /// Disposes this instance
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal void Dispose() {
    this._updateManager?.Dispose();
  }
}