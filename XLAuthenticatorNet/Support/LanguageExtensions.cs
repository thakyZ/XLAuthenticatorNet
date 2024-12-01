using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The language extensions class
/// </summary>
internal static class LanguageExtensions {
  /// <summary>
  /// Gets the language codes as a dictionary of <see cref="Lanuage" /> and the ISO code.
  /// </summary>
  /// <returns>A dictionary of <see cref="Lanuage" /> and language ISO code</returns>
  private static Dictionary<Language, string> GetLangCodes()
    // MUST match LauncherLanguage enum
    => new() {
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
  internal static string GetLocalizationCode(this Language? language)
    => LanguageExtensions.GetLangCodes()[language ?? Language.English]; // Default localization language

  /// <summary>
  /// Tests if the specified language is the default language
  /// </summary>
  /// <param name="language">The language to test.</param>
  /// <returns></returns>
  internal static bool IsDefault(this Language? language)
    => language is null or Language.English;

  /// <summary>
  /// Gets the language from two letter ISo identifier using the specified language
  /// </summary>
  /// <param name="_">The language parameter (unused)</param>
  /// <param name="code">The code</param>
  /// <returns>The language</returns>
  [SuppressMessage("Roslynator", "RCS1175:Unused 'this' parameter", Justification = "<Pending>")]
  internal static Language GetLangFromTwoLetterIso(this Language? _, string code)
    => GetLangCodes().Where(langCode => langCode.Value.Equals(code, StringComparison.OrdinalIgnoreCase)) is List<KeyValuePair<Language, string>> find && find.Count != 0 ? find[0].Key : Language.English; // Default language
}