using System.Text.RegularExpressions;
using CommandLine;
using Serilog.Enrichers.Sensitive;
using Serilog;
using Serilog.Enrichers.ShortTypeName;

namespace XLAuth.Support;

public static class LogInit {
  // ReSharper disable once ClassNeverInstantiated.Local
  private sealed class LogOptions {
    [Option('v', "verbose", Required = false, HelpText = "Set output to verbose messages.")]
    public bool Verbose { get; set; }

    [Option("log-file-path", Required = false, HelpText = "Set path for log file.")]
    public string? LogPath { get; set; }
  }

  public static void Setup(string defaultLogPath, string[] args) {
    ParserResult<LogOptions>? result = null;

    try {
      using var parser = new Parser(config => config.IgnoreUnknownArguments = true);
      result = parser.ParseArguments<LogOptions>(args);
#if DEBUG
    } catch {
      throw;
#else
    } catch {
      // Do nothing...
#endif
    }

    var config = new LoggerConfiguration();

    var parsed = result?.Value ?? new LogOptions();

    if (!string.IsNullOrEmpty(parsed.LogPath)) {
      config.WriteTo.Async(settings => settings.File(parsed.LogPath));
    } else {
      config.WriteTo.Async(settings => settings.File(defaultLogPath));
    }

#if DEBUG
    config.WriteTo.Debug();
    config.WriteTo.Console();
    config.MinimumLevel.Verbose();
#else
    config.MinimumLevel.ControlledBy(Logger.LoggingLevelSwitch);
#endif

    config.Enrich.WithShortTypeName();
    config.Enrich.WithDemystifiedStackTraces();
    config.Enrich.FromLogContext();
    config.Enrich.WithSensitiveDataMasking(options => options.MaskingOperators = [ new UserPathMaskingOperator() ]);

    if (parsed.Verbose) {
      config.MinimumLevel.Verbose();
    }

    Log.Logger = config.CreateLogger();
  }

  private sealed class UserPathMaskingOperator : RegexMaskingOperator {
    private const string _TEST_USER_PATH_PATTERN = @"(?<=C:[\\/]Users[\\/])\w+(?=[\\/])";

    public UserPathMaskingOperator()
        : base(_TEST_USER_PATH_PATTERN, RegexOptions.IgnoreCase | RegexOptions.Compiled) {
    }

    protected override bool ShouldMaskInput(string input) {
      return !string.Equals(input, "public", StringComparison.OrdinalIgnoreCase);
    }
  }
}
