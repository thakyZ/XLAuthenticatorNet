using System.Runtime.CompilerServices;
using System.Windows;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="ResourceDictionary" /> type.
/// </summary>
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
