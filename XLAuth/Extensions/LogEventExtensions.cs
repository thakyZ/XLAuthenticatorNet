using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Serilog.Events;

namespace XLAuth.Extensions;

/// <summary>
/// Extension methods for <see cref="LogEvent" />
/// </summary>
internal static class LogEventExtensions {
  public static string GetMessage(this LogEvent logEvent) {
    using var memoryStream = new MemoryStream();
    using var textWriter = new StreamWriter(memoryStream, Encoding.UTF8);
    logEvent.RenderMessage(textWriter, CultureInfo.InvariantCulture);
    return Encoding.UTF8.GetString(memoryStream.ToArray());
  }
}
