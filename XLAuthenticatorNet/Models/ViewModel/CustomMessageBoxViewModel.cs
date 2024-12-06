using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using XLAuthenticatorNet.Domain.Commands;
using XLAuthenticatorNet.Models.Abstracts;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The custom message box view model class
/// </summary>
/// <seealso cref="ViewModelBase{CustomMessageBox}"/>
internal sealed class CustomMessageBoxViewModel : ViewModelBase<CustomMessageBox> {
#region Binding
#region Title
  /// <summary>
  /// The internal value indicating the text of the <see cref="CustomMessageBox" /> <see cref="Window" /> title.
  /// </summary>
  private string? _caption = string.Empty;

  /// <summary>
  /// Gets or sets the value indicating the text of the <see cref="CustomMessageBox" /> <see cref="Window" /> title.
  /// </summary>
  public string? Caption {
    get => this._caption;
    set => this.SetProperty(ref this._caption, value);
  }
#endregion

#region Message
  /// <summary>
  /// The internal value indicating the text of the <see cref="CustomMessageBox" /> title <see cref="TextBlock" />.
  /// </summary>
  private string? _message;

  /// <summary>
  /// Gets or sets the value indicating the text of the <see cref="CustomMessageBox" /> title <see cref="TextBlock" />.
  /// </summary>
  public string? Message {
    get => this._message;
    set => this.SetProperty(ref this._message, value);
  }
#endregion

#region Description
  /// <summary>
  /// The internal value indicating the text of the <see cref="CustomMessageBox" /> description <see cref="TextBlock" />.
  /// </summary>
  private Visibility _descriptionVisibility = Visibility.Visible;

  /// <summary>
  /// Gets or sets the value indicating the text of the <see cref="CustomMessageBox" /> description <see cref="TextBlock" />.
  /// </summary>
  public Visibility DescriptionVisibility {
    get => this._descriptionVisibility;
    set => this.SetProperty(ref this._descriptionVisibility, value);
  }
#endregion

#region Icon
  /// <summary>
  /// The internal value indicating the <see cref="PackIconKind" /> of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  private PackIconKind _iconKind = PackIconKind.CloseOctagon;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="PackIconKind" /> of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  public PackIconKind IconKind {
    get => this._iconKind;
    set => this.SetProperty(ref this._iconKind, value);
  }

  /// <summary>
  /// The internal value indicating the <see cref="Brush" /> color of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  private Brush _iconColor = Brushes.Red;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Brush" /> color of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  public Brush IconColor {
    get => this._iconColor;
    set => this.SetProperty(ref this._iconColor, value);
  }

  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  private Visibility _iconVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> icon.
  /// </summary>
  public Visibility IconVisibility {
    get => this._iconVisibility;
    set => this.SetProperty(ref this._iconVisibility, value);
  }
#endregion

#region Dialog Buttons
#region OK Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> OK button.
  /// </summary>
  private Visibility _okButtonVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> OK button.
  /// </summary>
  public Visibility OKButtonVisibility {
    get => this._okButtonVisibility;
    set => this.SetProperty(ref this._okButtonVisibility, value);
  }

  /// <summary>
  /// Gets the value of the OK button command.
  /// </summary>
  public ICommand OKButtonCommand
    => new CommandImpl(() => this.Parent.SetResult(MessageBoxResult.OK));

  /// <summary>
  /// Gets the text value of the OK button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string OKButtonLabel
    => Loc.Localize(nameof(this.OKButtonLabel), "OK");
#endregion

#region Cancel Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> cancel button.
  /// </summary>
  private Visibility _cancelButtonVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> cancel button.
  /// </summary>
  public Visibility CancelButtonVisibility {
    get => this._cancelButtonVisibility;
    set => this.SetProperty(ref this._cancelButtonVisibility, value);
  }

  /// <summary>
  /// Gets the value of the cancel command
  /// </summary>
  public ICommand CancelButtonCommand
    => new CommandImpl(() => this.Parent.SetResult(MessageBoxResult.Cancel));

  /// <summary>
  /// Gets the text value of the cancel button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string CancelButtonLabel
    => Loc.Localize("CancelButtonLabel", "Cancel");
#endregion

#region Yes Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> yes button.
  /// </summary>
  private Visibility _yesButtonVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> yes button.
  /// </summary>
  public Visibility YesButtonVisibility {
    get => this._yesButtonVisibility;
    set => this.SetProperty(ref this._yesButtonVisibility, value);
  }

  /// <summary>
  /// Gets the value of the yes command
  /// </summary>
  public ICommand YesButtonCommand
    => new CommandImpl(() => this.Parent.SetResult(MessageBoxResult.Yes));

  /// <summary>
  /// Gets the text value of the yes button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string YesButtonLabel => Loc.Localize("YesButtonLabel", "Yes");
#endregion

#region No Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> no button.
  /// </summary>
  private Visibility _noVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> no button.
  /// </summary>
  public Visibility NoVisibility {
    get => this._noVisibility;
    set => this.SetProperty(ref this._noVisibility, value);
  }

  /// <summary>
  /// Gets the value of the no command
  /// </summary>
  public ICommand NoCommand => new CommandImpl(() => this.Parent.SetResult(MessageBoxResult.No));

  /// <summary>
  /// Gets the text value of the no button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string NoButtonLabel => Loc.Localize("NoButtonLabel", "No");
#endregion

#region Report Issue Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> report issue button.
  /// </summary>
  private Visibility _reportIssueVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> report issue button.
  /// </summary>
  public Visibility ReportIssueVisibility {
    get => this._reportIssueVisibility;
    set => this.SetProperty(ref this._reportIssueVisibility, value);
  }

  /// <summary>
  /// Gets the value of the report issue command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand ReportIssueCommand => new CommandImpl(() => {});

  /// <summary>
  /// Gets the text value of the report issue label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string ReportIssueLabel => Loc.Localize("ReportIssueLabel", "New GitHub Issue");
#endregion

#region Open FAQ Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> FAQ button.
  /// </summary>
  private Visibility _faqVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> FAQ button.
  /// </summary>
  public Visibility FAQVisibility {
    get => this._faqVisibility;
    set => this.SetProperty(ref this._faqVisibility, value);
  }

  /// <summary>
  /// Gets the value of the open FAQ command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand OpenFAQCommand => new CommandImpl(() => {});

  /// <summary>
  /// Gets the text value of the open FAQ label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string OpenFAQLabel => Loc.Localize("OpenFaqLabel", "Open FAQ");
#endregion

#region Join Discord Button
  /// <summary>
  /// The internal value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> join Discord button.
  /// </summary>
  private Visibility _discordVisibility = Visibility.Hidden;

  /// <summary>
  /// Gets or sets the value indicating the <see cref="Visibility" /> of the <see cref="CustomMessageBox" /> join Discord button.
  /// </summary>
  public Visibility DiscordVisibility {
    get => this._discordVisibility;
    set => this.SetProperty(ref this._discordVisibility, value);
  }

  /// <summary>
  /// Gets the value of the join discord button command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand JoinDiscordButtonCommand => new CommandImpl(() => {});

  /// <summary>
  /// Gets the text value of the join discord button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string JoinDiscordButtonLabel => Loc.Localize(nameof(this.JoinDiscordButtonLabel), "Join Discord");
#endregion

#region Copy Message Button
  /// <summary>
  /// Gets the value of the copy message text command
  /// </summary>
  public ICommand CopyMessageTextCommand => new CommandImpl(() => {
    if (this.Message is not null) {
      Clipboard.SetText(this.Message);
    }
  });
  /// <summary>
  /// Gets the text value of the copy message text label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string CopyMessageTextLabel => Loc.Localize("CopyMessageTextLabel", "Copy Message");
#endregion
#endregion
#endregion

  /// <summary>
  /// Initializes a new instance of the <see cref="CustomMessageBoxViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal CustomMessageBoxViewModel(CustomMessageBox parent) : base (parent) {}

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public CustomMessageBoxViewModel() {}
#endif
}
