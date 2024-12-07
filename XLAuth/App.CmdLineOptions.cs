using CommandLine;

#if !XL_NOAUTOUPDATE
using XLAuth.Update;
#endif

namespace XLAuth;

public partial class App {
  /// <summary>
  /// The cmd line options class
  /// </summary>
  public class CmdLineOptions {
    /// <summary>
    /// Gets or sets the value of the roaming path
    /// </summary>
    [Option("roamingPath", Required = false, HelpText = "Path to a folder to override the roaming path for XL with.")]
    public string? RoamingPath { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the no auto send OTP
    /// </summary>
    [Option("noAutoSendOTP", Required = false, HelpText = "Disable auto send OTP on start-up.")]
    public bool NoAutoSendOTP { get; set; }

    /// <summary>
    /// Gets or sets the value of the do generate localizables
    /// </summary>
    [Option("gen-localizable", Required = false, HelpText = "Generate localizable files.")]
    public bool DoGenerateLocalizables { get; set; }

    /// <summary>
    /// Gets or sets the value of the do generate integrity
    /// </summary>
    [Option("gen-integrity", Required = false, HelpText = "Generate integrity files. Provide a game path.")]
    public string? DoGenerateIntegrity { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the account name
    /// </summary>
    [Option("account", Required = false, HelpText = "Account name to use.")]
    public string? AccountName { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the language
    /// </summary>
    [Option("lang", Required = false, HelpText = "Language to use.")]
    public Language? Language { get; set; }

    // We don't care about these, just need it so that the parser doesn't error
    /// <summary>
    /// Gets or sets the value of the squirrel updated
    /// </summary>
    [Option("squirrel-updated", Hidden = true)]
    public string? SquirrelUpdated { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel install
    /// </summary>
    [Option("squirrel-install", Hidden = true)]
    public string? SquirrelInstall { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel obsolete
    /// </summary>
    [Option("squirrel-obsolete", Hidden = true)]
    public string? SquirrelObsolete { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel uninstall
    /// </summary>
    [Option("squirrel-uninstall", Hidden = true)]
    public string? SquirrelUninstall { get; set; } = null;

    /// <summary>
    /// Gets or sets the value of the squirrel first run
    /// </summary>
    [Option("squirrel-first-run", Hidden = true)]
    public bool SquirrelFirstRun { get; set; }
  }
}
