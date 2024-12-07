namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="Type" /> type.
/// </summary>
internal static class TypeExtensions {
  /// <summary>
  /// Checks if the provided <see cref="Type" /> is a subclass of a raw generic <see cref="Type" />.
  /// </summary>
  /// <param name="toCheck">The <see cref="Type" /> to check.</param>
  /// <param name="generic">The raw generic <see cref="Type" /> to check against.</param>
  /// <returns><see langword="true" /> if the provided <see cref="Type" /> is a subclass of the other; otherwise <see langword="false" />.</returns>
  public static bool IsSubclassOfRawGeneric(this Type? toCheck, Type generic) {
    while (toCheck is not null) {
      var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
      if (generic == cur) {
        return true;
      }
      toCheck = toCheck.BaseType;
    }
    return false;
  }
}
