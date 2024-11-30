using System;
using System.Linq;

namespace XLAuthenticatorNet.Domain;

/// <summary>
/// The api helpers class
/// </summary>
internal static class ApiHelpers {
  /// <summary>
  /// Generates the accept language using the specified asdf
  /// </summary>
  /// <param name="asdf">The asdf</param>
  /// <returns>The hdr</returns>
  internal static string GenerateAcceptLanguage(int asdf = 0) {
    string[] codes = ["de-DE", "en-US", "ja"];
    string[] codesMany = ["de-DE", "en-US,en", "en-GB,en", "fr-BE,fr", "ja", "fr-FR,fr", "fr-CH,fr"];
    var rng = new Random(asdf);
    bool many = rng.Next(10) < 3;
    if (!many) {
      return codes[rng.Next(0, codes.Length)];
    }

    int howMany = rng.Next(2, 4);
    string[] deck = codesMany.OrderBy(_ => rng.Next()).Take(howMany).ToArray();
    string hdr = string.Empty;
    for (var i = 0; i < deck.Length; i++) {
      hdr += deck.ElementAt(i) + $";q=0.{10 - (i + 1)}";
      if (i != deck.Length - 1) {
        hdr += ";";
      }
    }

    return hdr;
  }
}