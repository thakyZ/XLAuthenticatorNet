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
using XLAuthenticatorNet.Windows;
using Newtonsoft.Json.Linq;
using Serilog.Events;
using XLAuthenticatorNet.Extensions;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// Wrapper class for logging to the console, file, or debug.
/// </summary>
public partial class Logger {
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
    Logger.GetContext().Error(exception, messageTemplate, propertyValues);

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
    => new SerilogLoggerFactory(Log.Logger).CreateLogger(nameof(XLAuthenticatorNet) + "-Fallback");

  internal static SerilogILogger GetContext(int backwards = 2) {
    try {
      StackTrace stackTrace = new StackTrace();
      var frame = stackTrace.GetFrame(backwards);

      if (frame is null) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the frame {0}.", backwards);
        return Log.Logger;
      }

      if (!frame.HasMethod()) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the method on frame {0}.", backwards);
        return Log.Logger;
      }

      var frameMethod = frame.GetMethod();

      if (frameMethod is null) {
        SerilogContext.Error("Declaring type in stack trace couldn't find the method on frame {0}, instead got {1}.", backwards, frameMethod?.Name ?? "<null>");
        return Log.Logger;
      }

      var type = frameMethod.DeclaringType;

      if (type is null) {
        SerilogContext.Error("Declaring type in stack trace frame {0} couldn't find declaring type, instead got {1}.", backwards, type?.FullName ?? "<null>");
        return Log.Logger;
      }

      Type logType = typeof(Log);
      var method = logType.GetMethods().FirstOrDefault(method => method.Name.Equals("ForContext", StringComparison.Ordinal) && method.IsGenericMethod);

      if (method is null) {
        SerilogContext.Error("Methods in type {0} couldn't find method by name {1}, instead got {2}.", typeof(Log).FullName, nameof(Log.ForContext), method?.GetType().FullName ?? "<null>");
        return Log.Logger;
      }

      var @delegate = method.MakeGenericMethod([type]);
      var invoked = @delegate.Invoke(null, []);

      if (invoked is not SerilogILogger logger) {
        SerilogContext.Error("Delegate invoked was not type of {0}, instead got {1}.", typeof(SerilogILogger).FullName, invoked?.GetType().FullName ?? "<null>");
        return Log.Logger;
      }

      return logger;
    } catch (Exception exception) {
      SerilogContext.Error(exception, "Failed at Logger.GetContext");
      return Log.Logger;
    }
  }

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

      if (Log.Logger is null) {
        SerilogContext.Error("Typeof {0}.{1} is null, this shouldn't happen.", typeof(Log).FullName, nameof(Log.Logger));
        return MicrosoftFallbackContext;
      }

      var factory = new SerilogLoggerFactory(Log.Logger);

      Type factoryType = typeof(SerilogLoggerFactory);
      var method = factoryType.GetMethods().FirstOrDefault(method => method.Name.Equals("CreateLogger", StringComparison.Ordinal) && method.IsGenericMethod);

      if (method is null) {
        SerilogContext.Error("Methods in type {0} couldn't find method by name {1}, instead got {2}.", nameof(Log), nameof(SerilogLoggerFactory.CreateLogger), method?.GetType().FullName ?? "<null>");
        return MicrosoftFallbackContext;
      }

      var @delegate = method.MakeGenericMethod([type]);
      var invoked = @delegate.Invoke(null, []);

      if (invoked is not MicrosoftILogger logger) {
        SerilogContext.Error("Delegate invoked was not type of {0}, instead got {1}.", typeof(MicrosoftILogger).FullName, invoked?.GetType().FullName ?? "<null>");
        return MicrosoftFallbackContext;
      }

      return logger;
    } catch (Exception exception) {
      SerilogContext.Error(exception, "Failed at Logger.GetContext");
      return MicrosoftFallbackContext;
    }
  }
}
