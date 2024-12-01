using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Documents;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The collection extensions class
/// </summary>
internal static class CollectionExtensions {
  /// <summary>
  /// Removes the where using the specified collection
  /// </summary>
  /// <typeparam name="TSource">The </typeparam>
  /// <param name="collection">The collection</param>
  /// <param name="predicate">The func</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<TSource>(this ICollection<TSource> collection, Func<TSource, bool> predicate) {
    for (var index = 0; index < collection.Count; index++) {
      var item = collection.ElementAt(index);
      if (predicate(item)) {
        collection.Remove(item);
      }
    }
  }

  /// <summary>
  /// Filters two lists via combining <see cref="System.Linq.Enumerable.Where" /> and <see cref="System.Collections.Generic.List{T}.TrueForAll(Predicate{T})" />
  /// </summary>
  /// <typeparam name="TSourceA">Type that the type of <paramref name="source"/> inherits from.</typeparam>
  /// <typeparam name="TSourceB">Type that the type of <paramref name="other"/> inherits from.</typeparam>
  /// <param name="source">The main list to compare from.</param>
  /// <param name="other">The other list to compare to.</param>
  /// <param name="predicate">A function to compute the filter.</param>
  /// <returns>A collection filtered with func between two lists</returns>
  internal static IEnumerable<TSourceA> WhereIn<TSourceA, TSourceB>(this List<TSourceA> source, IEnumerable<TSourceB> other, Func<TSourceA, TSourceB, bool> predicate) {
    return source.Where(sourceA => other.All(sourceB => predicate(sourceA, sourceB)));
  }

  /// <summary>
  /// Removes the where using the specified list
  /// </summary>
  /// <typeparam name="TSource">The </typeparam>
  /// <param name="list">The list</param>
  /// <param name="predicate">The func</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<TSource>(this ObservableCollection<TSource> list, Func<TSource, bool> predicate) {
    for (var index = 0; index < list.Count; index++) {
      if (predicate(list[index])) {
        list.RemoveAt(index);
      }
    }
  }

  /// <summary>
  /// Removes the an item in the specified list
  /// </summary>
  /// <typeparam name="TSource">The type the list inherits from.</typeparam>
  /// <param name="list">The list to iterate over.</param>
  /// <param name="predicate">The predicate matching where an item should be removed.</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<TSource>(this List<TSource> list, Func<TSource, bool> predicate) {
    for (var index = 0; index < list.Count; index++) {
      if (predicate(list[index])) {
        list.RemoveAt(index);
      }
    }
  }

  /// <summary>
  /// A foreach loop as func function with func variable as index.
  /// </summary>
  /// <typeparam name="TSource">The type of the enumerable.</typeparam>
  /// <param name="enumerable">The enumerable to loop through</param>
  /// <param name="func">The function to use on every iteration</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  private static void Each<TSource>(this IEnumerable<TSource> enumerable, Action<TSource, int> func) {
    for (var index = 0; enumerable.Skip(index).Any(); index++) {
      var item = enumerable.ElementAt(index);
      func(item, index++);
    }
  }

//  /// <summary>
//  /// A foreach loop as func function.
//  /// </summary>
//  /// <typeparam name="TSource">The type of the enumerable.</typeparam>
//  /// <param name="enumerable">The enumerable to loop through</param>
//  /// <param name="func">The function to use on every iteration</param>
//  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
//  private static void Each<TSource>(this IEnumerable<TSource> enumerable, Action<TSource> func) {
//    for (var index = 0; enumerable.Skip(index).Any(); index++) {
//      var item = enumerable.ElementAt(index);
//      func(item);
//    }
//  }

  /// <summary>
  /// Merges two lists into a single dictionary.
  /// </summary>
  /// <typeparam name="TKey">The key type</typeparam>
  /// <typeparam name="TValue">The value type</typeparam>
  /// <param name="keys">The list of keys</param>
  /// <param name="values">The lust of values</param>
  /// <returns>The two lists combined into a dictionary.</returns>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  [SuppressMessage("Member Design", "AV1130:Return type in method signature should be an interface to an unchangeable collection", Justification = "The whole point is to allow it to still be modifiable.")]
  internal static IDictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<TKey> keys, IEnumerable<TValue> values) where TKey : notnull {
    Dictionary<TKey, TValue> dic = [];
    keys.Each((key, index) => {
      if (!dic.ContainsKey(key)) {
        var value = values.ElementAt(index);
        dic.Add(key, value);
      }
    });
    return dic;
  }
}
