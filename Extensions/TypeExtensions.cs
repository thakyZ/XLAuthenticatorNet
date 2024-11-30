using System;

namespace XLAuthenticatorNet.Extensions;

internal static class TypeExtensions {
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