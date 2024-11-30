using XLAuthenticatorNet.Domain;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The support links class
/// </summary>
internal static class SupportLinks
{
  /// <summary>
  /// Opens the discord
  /// </summary>
  internal static void OpenDiscord() => Util.OpenWebsite("https://discord.gg/3NMcUV5");

  /// <summary>
  /// Opens the faq
  /// </summary>
  internal static void OpenFaq() => Util.OpenWebsite("https://goatcorp.github.io/faq/");
}