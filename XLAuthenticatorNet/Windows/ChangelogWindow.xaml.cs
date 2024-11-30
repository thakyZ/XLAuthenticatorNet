using System;
using System.Diagnostics.CodeAnalysis;
using System.Media;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using Newtonsoft.Json;
using Serilog;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Windows;

/// <summary>
/// The changelog window class
/// </summary>
/// <seealso cref="Window"/>
public partial class ChangelogWindow : Window {
  /// <summary>
  /// Gets the value of the model
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Local")]
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
    InitializeComponent();
    _prerelease = prerelease;
    this.DataContext = new ChangelogWindowViewModel(this);

    Activate();
    Topmost = true;
    Topmost = false;
    Focus();
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
    LoadChangelog();
  }

  /// <summary>
  /// Shows the dialog
  /// </summary>
  public new void ShowDialog() {
    base.ShowDialog();
    LoadChangelog();
  }

  /// <summary>
  /// Loads the changelog
  /// </summary>
  /// <exception cref="JsonException">Failed to parse data from uri \"{_META_URL}\".</exception>
  private void LoadChangelog() {
    // ReSharper disable once AsyncVoidLambda
    Task.Run(action: async () => {
      try {
        using var client = new HttpClient();
        ReleaseMeta? response = JsonConvert.DeserializeObject<ReleaseMeta>(await client.GetStringAsync(_META_URL));
        if (response is null) {
          throw new JsonException($"Failed to parse data from uri \"{_META_URL}\".");
        }
        Dispatcher.Invoke(() => this.Model.ChangelogText = _prerelease ? response.PrereleaseVersion.Changelog : response.ReleaseVersion.Changelog);
      } catch (Exception ex) {
        Log.Error(ex, "Could not get changelog");
        Dispatcher.Invoke(callback: () => {
          this.Model.ChangelogText = this.Model.ChangelogLoadingErrorLabel;
        });
      }
    });
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