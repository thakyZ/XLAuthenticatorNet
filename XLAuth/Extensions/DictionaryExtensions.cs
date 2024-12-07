using System.Runtime.CompilerServices;
using System.Windows;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="Dictionary{TKey, TValue}" /> type.
/// </summary>
internal static class DictionaryExtensions {
  /// <summary>
  /// Returns the native dictionary using the specified resource dictionary
  /// </summary>
  /// <param name="resourceDictionary">The resource dictionary</param>
  /// <returns>The dictionary</returns>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static Dictionary<object, object?> ToNativeDictionary(this ResourceDictionary resourceDictionary) {
    Dictionary<object, object?> dictionary = [];
    foreach (var key in resourceDictionary.Keys) {
      dictionary[key] = resourceDictionary[key];
    }

    return dictionary;
  }
}
