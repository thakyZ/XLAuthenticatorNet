using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The http client extensions class
/// </summary>
internal static class HttpClientExtensions {
  /// <summary>
  /// Gets the from json using the specified http client
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="httpClient">The http client</param>
  /// <param name="url">The url</param>
  /// <param name="serializerSettings">The serializer settings</param>
  /// <param name="cancellationToken">The cancellation token</param>
  /// <exception cref="HttpRequestException">Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).</exception>
  /// <returns>A task containing the</returns>
  internal static async Task<T?> GetFromJsonAsync<T>(this HttpClient httpClient, string url, JsonSerializerSettings serializerSettings, CancellationToken cancellationToken = default) {
    HttpResponseMessage response = await httpClient.GetAsync(new Uri(url), cancellationToken);

    if (response.StatusCode != HttpStatusCode.OK) {
      throw new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode} ({response.StatusCode}).");
    }

    string jsonText = await response.Content.ReadAsStringAsync(cancellationToken);
    return JsonConvert.DeserializeObject<T>(jsonText, serializerSettings);
  }
}