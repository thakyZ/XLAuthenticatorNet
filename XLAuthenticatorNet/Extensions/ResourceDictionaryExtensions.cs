using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The resource dictionary extensions class
/// </summary>
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
internal static class ResourceDictionaryExtensions {
  /// <summary>
  /// Returns the list using the specified resource dictionary
  /// </summary>
  /// <param name="resourceDictionary">The resource dictionary</param>
  /// <returns>A list of key value pair object and object</returns>
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  internal static List<KeyValuePair<object, object?>> ToList(this ResourceDictionary resourceDictionary) {
    return [..resourceDictionary.Keys.Cast<object>().Merge(resourceDictionary.Values.Cast<object?>())];
  }
}