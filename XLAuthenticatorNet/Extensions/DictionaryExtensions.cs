using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Windows;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The dictionary extensions class
/// </summary>
internal static class DictionaryExtensions {
  /// <summary>
  /// Returns the native dictionary using the specified resource dictionary
  /// </summary>
  /// <param name="resourceDictionary">The resource dictionary</param>
  /// <returns>The dictionary</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static Dictionary<object, object?> ToNativeDictionary(this ResourceDictionary resourceDictionary) {
    Dictionary<object, object?> dictionary = [];
    foreach (var key in resourceDictionary.Keys) {
      dictionary[key] = resourceDictionary[key];
    }

    return dictionary;
  }
}