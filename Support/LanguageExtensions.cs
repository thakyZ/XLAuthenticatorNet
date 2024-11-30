using System.Collections.Generic;
using System.Linq;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The language extensions class
/// </summary>
internal static class LanguageExtensions {
  /// <summary>
  /// Gets the lang codes
  /// </summary>
  /// <returns>A dictionary of language and string</returns>
  private static Dictionary<Language, string> GetLangCodes()
    // MUST match LauncherLanguage enum
    => new Dictionary<Language, string> {
      { Language.Japanese, "ja" },
      { Language.English, "en" },
      { Language.German, "de" },
      { Language.French, "fr" },
      { Language.Italian, "it" },
      { Language.Spanish, "es" },
      { Language.Portuguese, "pt" },
      { Language.Korean, "ko" },
      { Language.Norwegian, "no" },
      { Language.Russian, "ru" },
      { Language.SimplifiedChinese, "zh" },
      { Language.TraditionalChinese, "tw" },
      { Language.Swedish, "sv" },
    };

  /// <summary>
  /// Gets the localization code using the specified language
  /// </summary>
  /// <param name="language">The language</param>
  /// <returns>The string</returns>
  internal static string GetLocalizationCode(this Language? language) => GetLangCodes()[language ?? Language.English]; // Default localization language

  /// <summary>
  /// Ises the default using the specified language
  /// </summary>
  /// <param name="language">The language</param>
  /// <returns>The bool</returns>
  internal static bool IsDefault(this Language? language) => language is null or Language.English;

  /// <summary>
  /// Gets the lang from two letter iso using the specified language
  /// </summary>
  /// <param name="language">The language</param>
  /// <param name="code">The code</param>
  /// <returns>The language</returns>
  internal static Language GetLangFromTwoLetterIso(this Language? language, string code)
    => GetLangCodes().Where(langCode => langCode.Value == code) is List<KeyValuePair<Language, string>> find && find.Count != 0 ? find[0].Key : Language.English; // Default language
}