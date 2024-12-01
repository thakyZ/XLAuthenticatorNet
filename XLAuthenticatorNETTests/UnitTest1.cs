using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;

namespace XLAuthenticatorNETTests;

[TestClass]
public class UnitTest1 {
  [TestMethod]
  public void Test1() {
    var result = TestLoggingClass.Run();
    Assert.IsTrue(result, "Failed to run");
  }

}

public static class Logger {
  public static bool Init() {
    try {
      var assemblyLocation = Assembly.GetExecutingAssembly().Location;
      Console.WriteLine($"assemblyLocation: {assemblyLocation}");
      var assemblyDirectory = new FileInfo(assemblyLocation).Directory;
      if (assemblyDirectory is null) {
        return false;
      }
      var logOutput = Path.Combine(assemblyDirectory.FullName, "output.log");
      if (logOutput is null) {
        return false;
      }
      Log.Logger = new LoggerConfiguration()
          .WriteTo.Async(settings => settings.File(logOutput))
          .WriteTo.Async(settings => settings.Debug()).MinimumLevel.Verbose()
          .CreateLogger();
      return true;
    } catch (Exception exception) {
      Log.Error(exception, "Failed at init");
    }
    return false;
  }

  public static bool Info(string message, params string[] arguments) {
    try {
      var initilized = Init();
      if (!initilized) {
        return false;
      }
      var context = Logger.GetContext();
      if (context is null) {
        return false;
      }
      context.Information("{0}", [message, ..arguments]);
      return true;
    } catch (Exception exception) {
      Log.Error(exception, "Failed at Info");
      return false;
    }
  }

  private static ILogger? GetContext() {
    try {
      StackTrace stackTrace = new StackTrace();
      const int STACK_TRACE_FRAME = 2;
      var frame = stackTrace.GetFrame(STACK_TRACE_FRAME);

      if (frame is null) {
        Log.Error("Declaring type in stack trace couldn't find the frame {0}.", STACK_TRACE_FRAME);
        return null;
      }

      if (!frame.HasMethod()) {
        Log.Error("Declaring type in stack trace couldn't find the method on frame {0}.", STACK_TRACE_FRAME);
        return null;
      }

      var frameMethod = frame.GetMethod();

      if (frameMethod is null) {
        Log.Error("Declaring type in stack trace couldn't find the method on frame {0}, instead got {1}.", STACK_TRACE_FRAME, frameMethod?.Name ?? "<null>");
        return null;
      }

      var type = frameMethod.DeclaringType;

      if (type is null) {
        Log.Error("Declaring type in stack trace frame {0} couldn't find declaring type, instead got {1}.", STACK_TRACE_FRAME, type?.FullName ?? "<null>");
        return null;
      }

      Type logType = typeof(Log);
      var method = logType.GetMethods().FirstOrDefault(method => method.Name.Equals("ForContext") && method.IsGenericMethod);

      if (method is null) {
        Log.Error("Methods in type {0} couldn't find method by name {1}, instead got {2}.", nameof(Log), nameof(Log.ForContext), method?.GetType().FullName ?? "<null>");
        return null;
      }

      var @delegate = method.MakeGenericMethod([type]);
      var invoked = @delegate.Invoke(null, []);

      if (invoked is not ILogger logger) {
        Log.Error("Delegate invoked was not type of {0}, instead got {1}.", nameof(ILogger), invoked?.GetType().FullName ?? "<null>");
        return null;
      }

      return logger;
    } catch (Exception exception) {
      Log.Error(exception, "Failed at GetContext");
      return null;
    }
  }
}

public static class TestLoggingClass {
  public static bool Run() {
    try {
      return Logger.Info("""
                         Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod
                         tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim
                         veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea
                         commodo consequat. Duis aute irure dolor in reprehenderit in voluptate
                         velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint
                         occaecat cupidatat non proident, sunt in culpa qui officia deserunt
                         mollit anim id est laborum.
                         """);
    } catch (Exception exception) {
      Log.Error(exception, "Failed at Run");
      return false;
    }
  }
}
