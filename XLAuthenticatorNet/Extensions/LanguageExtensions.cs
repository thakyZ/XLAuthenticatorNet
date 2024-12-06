namespace XLAuthenticatorNet.Extensions;

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
  /// Gets the localized language names as a dictionary of <see cref="Lanuage" /> and the localized language name.
  /// </summary>
  /// <returns>A dictionary of <see cref="Lanuage" /> and localized language name</returns>
  /// <remarks><para><seealso href="https://docs.translatehouse.org/projects/localization-guide/en/latest/l10n/languagenames.html" /></para></remarks>
  private static Dictionary<Language, string> GetLangNames()
    // MUST match LauncherLanguage enum
    => new() {
      { Language.Japanese, "日本語" },
      { Language.English, "English" },
      { Language.German, "Deutsch" },
      { Language.French, "Français" },
      { Language.Italian, "Italiano" },
      { Language.Spanish, "Español" },
      { Language.Portuguese, "Português" },
      { Language.Korean, "한국어" },
      { Language.Norwegian, "no" },
      { Language.Russian, "русский" },
      { Language.SimplifiedChinese, "简体中文" },
      { Language.TraditionalChinese, "繁體中文" },
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
  /// Gets the language from two letter ISO identifier using the specified language
  /// </summary>
  /// <param name="_">The language parameter (unused)</param>
  /// <param name="code">The code</param>
  /// <returns>The language</returns>
  [SuppressMessage("Roslynator", "RCS1175:Unused 'this' parameter", Justification = "<Pending>")]
  internal static Language GetLangFromTwoLetterIso(this Language? _, string code)
    => GetLangCodes().FirstOrDefault(langCode => langCode.Value.Equals(code, StringComparison.OrdinalIgnoreCase)).Key; // Default language (en) if default.

  /// <summary>
  /// Gets the localized language name using the specified language
  /// </summary>
  /// <param name="lanuage">The language parameter</param>
  /// <returns>The localized language name in the specified language.</returns>
  [SuppressMessage("Roslynator", "RCS1175:Unused 'this' parameter", Justification = "<Pending>")]
  internal static string GetName(this Language? lanuage)
    => GetLangNames().FirstOrDefault(keyValuePair => keyValuePair.Key.Equals(lanuage)).Value; // Default language (en) if default.
}
