using System.Collections.Generic;

namespace XLAuth.Support;

/// <summary>
/// The argument parser class
/// </summary>
internal static class ArgumentParser {
  /// <summary>
  /// Parses the arguments using the specified args
  /// </summary>
  /// <param name="args">The args</param>
  /// <returns>The output</returns>
  internal static List<ArgumentPair> ParseArguments(string[] args) {
    List<ArgumentPair> output = [];
    for (var i = 0; i < args.Length; i++) {
      if (i + 1 < args.Length && !args[i + 1].StartsWith('-')) {
        output.Add(new ArgumentPair<string>(args[i], args[i + 1]));
      } else if (args[i].Contains('=', System.StringComparison.Ordinal)) {
        string[] split = args[i].Split("=");
        output.Add(new ArgumentPair<string>(split[0], split[1]));
      } else {
        output.Add(new ArgumentPair<bool>(args[i], value: true));
      }
    }

    return output;
  }
}
