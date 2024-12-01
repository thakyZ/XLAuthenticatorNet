using System.Diagnostics;
using System.Web;
using System.Windows.Input;
using CheapLoc;
using Microsoft.Win32;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Domain.Commands;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Models.Abstracts;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The changelog window view model class
/// </summary>
/// <seealso cref="ViewModelBase{ChangelogWindow}"/>
internal sealed class ChangelogWindowViewModel : ViewModelBase<ChangelogWindow> {
#region Binding
#region Localizations
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Local")]
  public string Title
    => Loc.Localize(nameof(ChangelogWindow) + nameof(this.Title), "Changelog");

  /// <summary>
  /// Gets the value of the changelog loading error label
  /// </summary>
  public string ChangelogLoadingErrorLabel
    => Loc.Localize(nameof(this.ChangelogLoadingErrorLabel), "Could not load changelog. Please check GitHub or Discord instead!");

  /// <summary>
  /// Gets the value of the update notice label
  /// </summary>
  public string UpdateNoticeLabel
    => Loc.Localize(nameof(this.UpdateNoticeLabel), "XIVLauncher has been updated to version {0}.");

  /// <summary>
  /// Gets the value of the OK button label
  /// </summary>
  public string OKButtonLabel
    => Loc.Localize(nameof(this.OKButtonLabel), "OK");

  /// <summary>
  /// Gets the value of the send email button label
  /// </summary>
  public string SendEmailButtonLabel
    => Loc.Localize(nameof(this.SendEmailButtonLabel), "Send Email");

  /// <summary>
  /// Gets the value of the join discord button label
  /// </summary>
  public string JoinDiscordButtonLabel
    => Loc.Localize(nameof(this.JoinDiscordButtonLabel), "Join Discord");

  /// <summary>
  /// Gets the value of the email info text
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string EmailInfoText
    => Loc.Localize(nameof(this.EmailInfoText),
      """
      XL Authenticator .NET is free, open-source software - it doesn't use any telemetry or analysis tools to collect
      your data, but it would help a lot if you could send me a short email with your operating system, why you use
      XL Authenticator .NET and, if needed, any criticism or things we can do better. Your email will be deleted
      immediately after evaluation and I won't ever contact you.\n\nThank you very much for using XL Authenticator .NET!
      """.CompactMultilineString());

  /// <summary>
  /// Gets the value of the changelog thanks label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string ChangelogThanksLabel => Loc.Localize("ChangelogThanks", "XL Authenticator .NET is free, open-source software supported by a variety of people from all over the world.\nThank you for sticking around!");
#endregion Localizations

#region Text Properties
  /// <summary>
  /// The internal update notice text
  /// </summary>
  private string _updateNoticeText = "N/A";

  /// <summary>
  /// Gets or sets the value of the update notice text
  /// </summary>
  public string UpdateNoticeText {
    get => this._updateNoticeText;
    set => this.SetProperty(ref this._updateNoticeText, value);
  }

  /// <summary>
  /// The internal changelog text
  /// </summary>
  private string _changelogText = "N/A";

  /// <summary>
  /// Gets or sets the value of the changelog text
  /// </summary>
  public string ChangelogText {
    get => this._changelogText;
    set => this.SetProperty(ref this._changelogText, value);
  }
#endregion Text Properties

#region Commands
  /// <summary>
  /// Gets the value of the join discord button command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand JoinDiscordButtonCommand
    => new CommandImpl(SupportLinks.OpenDiscord);

  /// <summary>
  /// Gets the value of the send email button command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand SendEmailButtonCommand
    => new CommandImpl(() => {
      // Try getting the Windows 10 "build", e.g. 1909
      var releaseId = "???";
      try {
        releaseId = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion", "ReleaseId", "")?.ToString();
      } catch {
        // ignored
      }

      string? os   = HttpUtility.HtmlEncode($"{Environment.OSVersion} - {releaseId} ({Environment.Version})");
      string? lang = HttpUtility.HtmlEncode(App.Settings.Language.GetValueOrDefault(Language.English).ToString());
      string? wine = EnvironmentSettings.IsWine ? "Yes" : "No";

      // ReSharper disable once UseStringInterpolation
      var emailString = ResourceHelpers.GetFromResources<string>("email_to");
      if (emailString is not null) {
        Process.Start(string.Format(CultureInfo.InvariantCulture, emailString, os, lang, wine));
      } else {
        Logger.Warning("Failed to get resource by key 'email_to'.");
      }
    });

  /// <summary>
  /// Gets the value of the close button command
  /// </summary>
  public ICommand CloseButtonCommand
    => new CommandImpl(this.Parent.Close);
#endregion Commands
#endregion Bindings

#region Constructors
  /// <summary>
  /// Initializes a new instance of the <see cref="ChangelogWindowViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal ChangelogWindowViewModel(ChangelogWindow parent) : base(parent) {}

#if DEBUG
//[SuppressMessage("ReSharper", "UnusedMember.Global"),
// SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
// SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public ChangelogWindowViewModel() {}
#endif
#endregion Constructors
}
