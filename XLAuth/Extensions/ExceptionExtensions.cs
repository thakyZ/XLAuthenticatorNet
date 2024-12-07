using System.Runtime.CompilerServices;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="Exception" /> type.
/// </summary>
internal static class ExceptionExtensions {
  /// <summary>
  /// Throws an <see cref="ArgumentOutOfRangeException" /> if a enum that was parsed out of range.
  /// </summary>
  /// <typeparam name="TOut">A type parameter for output into switch expressions.</typeparam>
  /// <param name="variableName">The name of the enum variable.</param>
  /// <param name="value">The value of the enum variable.</param>
  /// <param name="message">An optional message.</param>
  /// <returns>Always <see langword="null" />.</returns>
  [DoesNotReturn]
  [MethodImpl(MethodImplOptions.AggressiveInlining)]
  public static TOut ThrowEnumOutOfRangeException<TOut, TEnum>(string variableName, TEnum? value, string? message = null) where TEnum : Enum {
    throw new ArgumentOutOfRangeException(variableName, value, message);
  }

  public static string ToFullyQualifiedString<TException>(this TException exception) where TException : Exception {
    var sb = new StringBuilder()
      .Append('[')
      .Append(typeof(TException).Name)
      .Append(']')
      .Append(' ')
      .AppendLine(exception.Message);
    if (exception.HelpLink is string helpLink) {
      sb.Append("Get help at: ")
        .AppendLine(helpLink);
    }
    if (exception.StackTrace is string stackTrace) {
      sb.AppendLine(stackTrace);
    }
    if (exception.InnerException is not null) {
      var innerException = exception.ToFullyQualifiedString();
      sb.AppendLine(innerException);
    }
    return sb.ToString();
  }
}
