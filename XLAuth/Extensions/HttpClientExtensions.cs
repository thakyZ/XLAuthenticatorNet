using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XLAuth.Extensions;

/// <summary>
/// The HTTP client extensions class
/// </summary>
internal static class HttpClientExtensions {
  /// <summary>
  /// Gets the from json using the specified HTTP client
  /// </summary>
  /// <typeparam name="TOut">The </typeparam>
  /// <param name="httpClient">The HTTP client</param>
  /// <param name="url">The url</param>
  /// <param name="serializerSettings">The serializer settings</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <exception cref="HttpRequestException">Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).</exception>
  /// <returns>A task containing the</returns>
  internal static async Task<TOut?> GetFromJsonAsync<TOut>(this HttpClient httpClient, string url, JsonSerializerSettings serializerSettings, CancellationToken cancellationToken = default) where TOut : class {
    var response = await httpClient.GetAsync(new Uri(url), cancellationToken).ConfigureAwait(false);

    if (response.StatusCode != HttpStatusCode.OK) {
      throw new HttpRequestException($"Response status code does not indicate success: {string.Create(CultureInfo.InvariantCulture, $"{(int)response.StatusCode}")} ({response.StatusCode}).");
    }

    var jsonText = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
    return JsonConvert.DeserializeObject<TOut>(jsonText, serializerSettings);
  }
}
