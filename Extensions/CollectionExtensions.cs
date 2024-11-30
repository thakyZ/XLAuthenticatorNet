using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace XLAuthenticatorNet.Extensions;

/// <summary>
/// The collection extensions class
/// </summary>
internal static class CollectionExtensions {
  /// <summary>
  /// Removes the where using the specified collection
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="collection">The collection</param>
  /// <param name="predicate">The predicate</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<T>(this ICollection<T> collection, Func<T, bool> predicate) {
    collection.Where(predicate).Each(i => collection.Remove(i));
  }

  /// <summary>
  /// Removes the where using the specified list
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="list">The list</param>
  /// <param name="predicate">The predicate</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<T>(this ObservableCollection<T> list, Func<T, bool> predicate) {
    for (var i = 0; i < list.Count; i++) {
      if (predicate(list[i])) {
        list.RemoveAt(i);
      }
    }
  }

  /// <summary>
  /// Removes the where using the specified list
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="list">The list</param>
  /// <param name="predicate">The predicate</param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static void RemoveWhere<T>(this List<T> list, Func<T, bool> predicate) {
    for (var i = 0; i < list.Count; i++) {
      if (predicate(list[i])) {
        list.RemoveAt(i);
      }
    }
  }

  /// <summary>
  /// Eaches the enumerable
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="enumerable">The enumerable</param>
  /// <param name="a">The </param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  private static void Each<T>(this IEnumerable<T> enumerable, Action<T, int> a) {
    var i = 0;
    foreach (T e in enumerable) {
      a(e, i++);
    }
  }

  /// <summary>
  /// Eaches the enumerable
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="enumerable">The enumerable</param>
  /// <param name="a">The </param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  private static void Each<T>(this List<T> enumerable, Action<T, int> a) {
    var i = 0;
    foreach (T e in enumerable) {
      a(e, i++);
    }
  }

  /// <summary>
  /// Eaches the enumerable
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="enumerable">The enumerable</param>
  /// <param name="a">The </param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  private static void Each<T>(this IEnumerable<T> enumerable, Action<T> a) {
    foreach (T e in enumerable) {
      a(e);
    }
  }

  /// <summary>
  /// Eaches the list
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="list">The list</param>
  /// <param name="a">The </param>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  private static void Each<T>(this List<T> list, Action<T> a) {
    // ReSharper disable once ForCanBeConvertedToForeach
    for (var i = 0; i < list.Count; i++) {
      a(list[i]);
    }
  }

  /// <summary>
  /// Merges the keys
  /// </summary>
  /// <typeparam name="TKey">The key</typeparam>
  /// <typeparam name="TValue">The value</typeparam>
  /// <param name="keys">The keys</param>
  /// <param name="values">The values</param>
  /// <returns>The dic</returns>
  [MethodImpl(MethodImplOptions.AggressiveOptimization)]
  internal static Dictionary<TKey, TValue> Merge<TKey, TValue>(this IEnumerable<TKey> keys, IEnumerable<TValue> values) where TKey : notnull {
    Dictionary<TKey, TValue> dic = [];
    keys.Each((x, i) => {
      if (!dic.ContainsKey(x)) {
        dic.Add(x, values.ElementAt(i));
      }
    });
    return dic;
  }
}