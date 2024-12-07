using System.IO;
using Serilog.Events;

namespace XLAuth.Extensions;

/// <summary>
/// An extension class for the <see cref="LogEvent" /> type.
/// </summary>
internal static class LogEventExtensions {
  public static string GetMessage(this LogEvent logEvent) {
    using var memoryStream = new MemoryStream();
    using var textWriter = new StreamWriter(memoryStream, Encoding.UTF8);
    logEvent.RenderMessage(textWriter, CultureInfo.InvariantCulture);
    return Encoding.UTF8.GetString(memoryStream.ToArray());
  }
}
