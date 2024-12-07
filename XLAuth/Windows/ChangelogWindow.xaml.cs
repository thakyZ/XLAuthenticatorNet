using System;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using XLAuth.Models.ViewModel;

namespace XLAuth.Windows;

/// <summary>
/// The changelog window class
/// </summary>
/// <seealso cref="Window"/>
public partial class ChangelogWindow : Window {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  private ChangelogWindowViewModel Model => (this.DataContext as ChangelogWindowViewModel)!;
  /// <summary>
  /// The prerelease
  /// </summary>
  private readonly bool _prerelease;
  /// <summary>
  /// The meta url
  /// </summary>
  private const string _META_URL = "https://api.nekogaming.xyz/ffxiv/xl-auth/Meta";

  /// <summary>
  /// Initializes a new instance of the <see cref="ChangelogWindow"/> class
  /// </summary>
  /// <param name="prerelease">The prerelease</param>
  internal ChangelogWindow(bool prerelease) {
    this.InitializeComponent();
    this._prerelease = prerelease;
    this.DataContext = new ChangelogWindowViewModel(this);

    this.Activate();
    this.Topmost = true;
    this.Topmost = false;
    this.Focus();
  }

  /// <summary>
  /// Updates the version using the specified version
  /// </summary>
  /// <param name="version">The version</param>
  internal void UpdateVersion(string version) {
    this.Model.UpdateNoticeText = string.Format(this.Model.UpdateNoticeLabel, version);
  }

  /// <summary>
  /// Shows this instance
  /// </summary>
  public new void Show() {
    SystemSounds.Asterisk.Play();
    base.Show();
    this.LoadChangelog();
  }

  /// <summary>
  /// Shows the dialog
  /// </summary>
  public new void ShowDialog() {
    base.ShowDialog();
    this.LoadChangelog();
  }

  /// <summary>
  /// Loads the changelog
  /// </summary>
  /// <exception cref="JsonException">Failed to parse data from uri \"{_META_URL}\".</exception>
  private void LoadChangelog() {
    // ReSharper disable once AsyncVoidLambda
    _ = Task.Run(async () => {
      try {
        ReleaseMeta response = JsonConvert.DeserializeObject<ReleaseMeta>(await App.HttpClient.GetStringAsync(_META_URL)) ?? throw new JsonException($"Failed to parse data from uri \"{_META_URL}\".");

        _ = this.Dispatcher.InvokeAsync(() => this.Model.ChangelogText = this._prerelease ? response.PrereleaseVersion.Changelog : response.ReleaseVersion.Changelog);
      } catch (Exception ex) {
        Logger.Error(ex, "Could not get changelog");
        _ = this.Dispatcher.InvokeAsync(() => this.Model.ChangelogText = this.Model.ChangelogLoadingErrorLabel);
      }
    })
#if DEBUG
    .ContinueWith((Task result) => {
      // Disable BCC4003
      Logger.Debug("Task [{0}] {1}: {2}={3}, {4}={5}, {6}={7}, {8}={9}",
        result.Id, nameof(LoadChangelog),
        nameof(Task.IsCanceled), result.IsCanceled,
        nameof(Task.IsCompleted), result.IsCompleted,
        nameof(Task.IsCompletedSuccessfully), result.IsCompletedSuccessfully,
        nameof(Task.IsFaulted), result.IsFaulted
      );
      if (result.Exception is not null) {
        Logger.Error(result.Exception, "Failed on method {0}", nameof(LoadChangelog));
      }
    })
#endif
    ;
  }

  /// <summary>
  /// The version meta class
  /// </summary>
  public class VersionMeta {
    /// <summary>
    /// Gets or sets the value of the version
    /// </summary>
    [JsonProperty("version")]
    public required string Version { get; set; }

    /// <summary>
    /// Gets or sets the value of the url
    /// </summary>
    [JsonProperty("url")]
    public required string Url { get; set; }

    /// <summary>
    /// Gets or sets the value of the changelog
    /// </summary>
    [JsonProperty("changelog")]
    public required string Changelog { get; set; }

    /// <summary>
    /// Gets or sets the value of the when
    /// </summary>
    [JsonProperty("when")]
    public required DateTime When { get; set; }
  }

  /// <summary>
  /// The release meta class
  /// </summary>
  public class ReleaseMeta {
    /// <summary>
    /// Gets or sets the value of the release version
    /// </summary>
    [JsonProperty("releaseVersion")]
    public required VersionMeta ReleaseVersion { get; set; }

    /// <summary>
    /// Gets or sets the value of the prerelease version
    /// </summary>
    [JsonProperty("prereleaseVersion")]
    public required VersionMeta PrereleaseVersion { get; set; }
  }
}
