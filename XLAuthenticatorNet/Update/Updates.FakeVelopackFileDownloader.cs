using System.IO;
using System.Net.Http;
using System.Threading;
using Velopack.Sources;

namespace XLAuthenticatorNet.Update;

internal sealed partial class Updates {
  /// <summary>
  /// A class to make a Velopack File Downloader Implementation.
  /// </summary>
  /// <seealso cref="IFileDownloader"/>
  private sealed class FakeVelopackFileDownloader : IFileDownloader {
    /// <summary>
    /// The lease
    /// </summary>
    private readonly Lease _lease;

    /// <summary>
    /// The HTTP client
    /// </summary>
    private readonly HttpClient _client = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="FakeVelopackFileDownloader"/> class.
    /// </summary>
    /// <param name="lease">The lease to use to download.</param>
    /// <param name="prerelease">A <see langword="bool" /> indicating to download a pre-release.</param>
    internal FakeVelopackFileDownloader(Lease lease, bool prerelease) {
      this._lease = lease;
      if (!this._client.DefaultRequestHeaders.TryAddWithoutValidation("X-XL-Track",
            prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE)) {
        Support.Logger.Warning("Failed to add header \"X-XL-Track\" with value {Track} to the HTTP Client's default headers.",
          prerelease ? _TRACK_PRERELEASE : _TRACK_RELEASE);
      }
    }

    /// <summary>
    /// Downloads the file using the specified url
    /// </summary>
    /// <param name="url">The url of the file to download.</param>
    /// <param name="targetFile">The target file</param>
    /// <param name="progress">The progress</param>
    /// <param name="authorization">The authorization</param>
    /// <param name="accept">To auto </param>
    public async Task DownloadFile(string url, string targetFile, Action<int> progress, string? authorization = null, string? accept = null, CancellationToken cancelToken = default) {
      Support.Logger.Verbose("FakeVelopack: DownloadFile from {Url} to {Target}", url, targetFile);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      using HttpResponseMessage response = await this._client.GetAsync($"{_LEASE_FILE_URL}/{fileNeeded}", HttpCompletionOption.ResponseHeadersRead, cancelToken).ConfigureAwait(false);
      response.EnsureSuccessStatusCode();
      Stream contentStream = await response.Content.ReadAsStreamAsync(cancelToken).ConfigureAwait(false);
      await using (contentStream.ConfigureAwait(false)) {
        Stream fileStream = File.Open(targetFile, FileMode.Create);
        await using (fileStream.ConfigureAwait(false)) {
          await contentStream.CopyToAsync(fileStream, cancelToken).ConfigureAwait(false);
          fileStream.Close();
          Support.Logger.Verbose("FakeVelopack: OK, downloaded {NumBytes}b for {File}", response.Content.Headers.ContentLength, fileNeeded);
        }
      }
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
      Support.Logger.Verbose("FakeVelopack: DownloadUrl from {Url}", url);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      if (fileNeeded.StartsWith("RELEASES", StringComparison.Ordinal)) {
        return Task.FromResult(Encoding.Default.GetBytes(this._lease.ReleasesList ?? string.Empty));
      }
      throw new ArgumentException($"DownloadUrl called for unknown file: {url}", nameof(url));
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
      Support.Logger.Verbose("FakeVelopack: DownloadUrl from {Url}", url);
      string fileNeeded = url[_FAKE_URL_PREFIX.Length..];
      if (fileNeeded.StartsWith("RELEASES", StringComparison.Ordinal)) {
        return Task.FromResult(this._lease.ReleasesList ?? string.Empty);
      }
      throw new ArgumentException($"DownloadUrl called for unknown file: {url}", nameof(url));
    }
  }
}
