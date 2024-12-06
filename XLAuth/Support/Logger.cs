using System;
using System.Diagnostics;
using System.Linq;
using Log = Serilog.Log;
using SerilogILogger = Serilog.ILogger;
using MicrosoftILogger = Microsoft.Extensions.Logging.ILogger;
using Serilog.Extensions.Logging;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using System.Windows;
using XLAuth.Windows;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using XLAuth.Extensions;
using System.Collections.Concurrent;

namespace XLAuth.Support;

/// <summary>
/// Wrapper class for logging to the console, file, or debug.
/// </summary>
public partial class Logger : IDisposable {
  private bool _isDisposed;

  /// <summary>
  /// Gets the value of the logging level switch
  /// </summary>
  private static ConcurrentDictionary<string, SerilogILogger> LoggerCache => [];

  /// <summary>
  /// Gets the value of the logging level switch
  /// </summary>
  internal static LoggingLevelSwitch LoggingLevelSwitch => new();

  /// <summary>
  /// An instance of <see cref="Serilog.ILogger" /> for logging to this class via <see cref="Serilog" />.
  /// </summary>
  private static SerilogILogger SerilogContext
    => Log.ForContext<Logger>();

  /*
   * Unused for now.
  /// <summary>
  /// An instance of <see cref="Microsoft.Extensions.Logging.ILogger" /> for logging to this class via <see cref="Microsoft.Extensions.Logging" />.
  /// </summary>
  private static MicrosoftILogger MicrosoftContext
    => new SerilogLoggerFactory(Log.Logger).CreateLogger<Logger>();
  */

  /// <summary>
  /// Write a log event with the <see cref="LogEventLevel.Error"/> level and associated exception.
  /// </summary>
  /// <param name="exception">Exception related to the event.</param>
  /// <param name="messageTemplate">Message template describing the event.</param>
  /// <param name="propertyValues">Objects positionally formatted into the message template.</param>
  /// <example><c>Log.Error(ex, "Failed {ErrorCount} records.", brokenRecords.Length);</c></example>
  [MessageTemplateFormatMethod("messageTemplate")]
  [SuppressMessage("CodeQuality", "Serilog004:Constant MessageTemplate verifier")]
  public static void ErrorDialog(Exception? exception, string messageTemplate, params object?[]? propertyValues) {
    Logger.Error(exception, messageTemplate, propertyValues);

    // Catch a common pitfall when a single non-object array is cast to object[]
    if (propertyValues is not null && propertyValues is not object[]) {
      propertyValues = [propertyValues];
    }

    if (Log.BindMessageTemplate(messageTemplate, propertyValues ?? [], out var parsedTemplate, out var boundProperties)) {
      var currentActivity = Activity.Current;
      var logEvent = new LogEvent(DateTimeOffset.Now, LogEventLevel.Error, exception, parsedTemplate, boundProperties, currentActivity?.TraceId ?? default, currentActivity?.SpanId ?? default);
      var message = logEvent.GetMessage();
      CustomMessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  /// <summary>
  /// An instance of <see cref="Microsoft.Extensions.Logging.ILogger" /> to provide fallback logging to this class via <see cref="Microsoft.Extensions.Logging" />.
  /// </summary>
  private static MicrosoftILogger MicrosoftFallbackContext
    => new SerilogLoggerFactory(Log.Logger).CreateLogger(nameof(XLAuth) + "-Fallback");

  internal static MicrosoftILogger GetMicrosoftContext(int backwards = 2) {
    try {
      StackTrace stackTrace = new StackTrace();
      var frame = stackTrace.GetFrame(backwards);

      if (frame is null) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the frame {0}.", backwards);
        return MicrosoftFallbackContext;
      }
      if (!frame.HasMethod()) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the method on frame {0}.", backwards);
        return MicrosoftFallbackContext;
      }

      var frameMethod = frame.GetMethod();

      if (frameMethod is null) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the method on frame {0}, instead got {1}.", backwards, frameMethod?.Name ?? "<null>");
        return MicrosoftFallbackContext;
      }

      var type = frameMethod.DeclaringType;

      if (type is null) {
        SerilogContext.Error("Declaring type in stack trace frame {0} couldn't find declaring type, instead got {1}.", backwards, type?.FullName ?? "<null>");
        return MicrosoftFallbackContext;
      }

      return new SerilogLoggerFactory(Log.Logger).CreateLogger(type);
    } catch (Exception exception) {
      SerilogContext.Error(exception, "Failed at Logger.GetContext");
      return MicrosoftFallbackContext;
    }
  }

  protected virtual void Dispose(bool disposing) {
    if (!_isDisposed) {
      if (disposing) {
        // NOTE: dispose managed state (managed objects)
      }

      // NOTE: free unmanaged resources (unmanaged objects) and override finalizer
      // NOTE: set large fields to null
      _isDisposed = true;
    }
  }

  // NOTE: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
  ~Logger() {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose(disposing: false);
  }

  public void Dispose() {
    // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }
}
