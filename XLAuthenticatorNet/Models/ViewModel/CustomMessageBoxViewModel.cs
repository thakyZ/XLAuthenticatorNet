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
internal class CustomMessageBoxViewModel : ViewModelBase<CustomMessageBox> {
#region Binding
#region Title
  /// <summary>
  /// The empty
  /// </summary>
  private string? _caption = string.Empty;
  /// <summary>
  /// Gets or sets the value of the caption
  /// </summary>
  public string? Caption {
    get => this._caption;
    set => this.SetProperty(ref this._caption, value);
  }
#endregion
#region Message
  /// <summary>
  /// The message
  /// </summary>
  private string? _message;
  /// <summary>
  /// Gets or sets the value of the message
  /// </summary>
  public string? Message {
    get => _message;
    set => this.SetProperty(ref _message, value);
  }
#endregion
#region Description
  /// <summary>
  /// The visibility of the description box
  /// </summary>
  private Visibility _descriptionVisibility = Visibility.Visible;

  /// <summary>
  /// Gets or sets the value of the description visibility
  /// </summary>
  public Visibility DescriptionVisibility {
    get => this._descriptionVisibility;
    set => this.SetProperty(ref this._descriptionVisibility, value);
  }
#endregion
#region Icon
/// <summary>
/// The default dialog icon
/// </summary>
private PackIconKind _iconKind = PackIconKind.CloseOctagon;

/// <summary>
/// Gets or sets the value of the icon kind
/// </summary>
public PackIconKind IconKind {
  get => _iconKind;
  set => this.SetProperty(ref _iconKind, value);
}

/// <summary>
/// The default dialog icon color
/// </summary>
private Brush _iconColor = Brushes.Red;

/// <summary>
/// Gets or sets the value of the icon color
/// </summary>
public Brush IconColor {
  get => _iconColor;
  set => this.SetProperty(ref _iconColor, value);
}
/// <summary>
/// The hidden
/// </summary>
private Visibility _iconVisibility = Visibility.Hidden;
/// <summary>
/// Gets or sets the value of the icon visibility
/// </summary>
public Visibility IconVisibility {
  get => _iconVisibility;
  set => this.SetProperty(ref _iconVisibility, value);
}
#endregion
#region Dialog Buttons
#region OK Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _okVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the ok visibility
  /// </summary>
  public Visibility OKVisibility {
    get => this._okVisibility;
    set => this.SetProperty(ref this._okVisibility, value);
  }
  /// <summary>
  /// Gets the value of the ok command
  /// </summary>
  public ICommand OKCommand => new CommandImpl(() => {
    this.Parent.SetResult(MessageBoxResult.OK);
  });
  /// <summary>
  /// Gets the value of the ok button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string OKButtonLabel => Loc.Localize(nameof(OKButtonLabel), "OK");
#endregion
#region Cancel Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _cancelVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the cancel visibility
  /// </summary>
  public Visibility CancelVisibility {
    get => this._cancelVisibility;
    set => this.SetProperty(ref this._cancelVisibility, value);
  }
  /// <summary>
  /// Gets the value of the cancel command
  /// </summary>
  public ICommand CancelCommand => new CommandImpl(() => {
    this.Parent.SetResult(MessageBoxResult.Cancel);
  });
  /// <summary>
  /// Gets the value of the cancel button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string CancelButtonLabel => Loc.Localize("CancelButtonLabel", "Cancel");
#endregion
#region Yes Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _yesVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the yes visibility
  /// </summary>
  public Visibility YesVisibility {
    get => this._yesVisibility;
    set => this.SetProperty(ref this._yesVisibility, value);
  }
  /// <summary>
  /// Gets the value of the yes command
  /// </summary>
  public ICommand YesCommand => new CommandImpl(() => {
    this.Parent.SetResult(MessageBoxResult.Yes);
  });
  /// <summary>
  /// Gets the value of the yes button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string YesButtonLabel => Loc.Localize("YesButtonLabel", "Yes");
#endregion
#region No Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _noVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the no visibility
  /// </summary>
  public Visibility NoVisibility {
    get => this._noVisibility;
    set => this.SetProperty(ref this._noVisibility, value);
  }
  /// <summary>
  /// Gets the value of the no command
  /// </summary>
  public ICommand NoCommand => new CommandImpl(() => {
    this.Parent.SetResult(MessageBoxResult.No);
  });
  /// <summary>
  /// Gets the value of the no button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string NoButtonLabel => Loc.Localize("NoButtonLabel", "No");
#endregion
#region Report Issue Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _reportIssueVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the report issue visibility
  /// </summary>
  public Visibility ReportIssueVisibility {
    get => _reportIssueVisibility;
    set => this.SetProperty(ref _reportIssueVisibility, value);
  }
  /// <summary>
  /// Gets the value of the report issue command
  /// </summary>
  public ICommand ReportIssueCommand => new CommandImpl(() => {});
  /// <summary>
  /// Gets the value of the report issue label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string ReportIssueLabel => Loc.Localize("ReportIssueLabel", "New GitHub Issue");
#endregion
#region Open FAQ Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _faqVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the faq visibility
  /// </summary>
  public Visibility FAQVisibility {
    get => _faqVisibility;
    set => this.SetProperty(ref _faqVisibility, value);
  }
  /// <summary>
  /// Gets the value of the open faq command
  /// </summary>
  public ICommand OpenFAQCommand => new CommandImpl(() => {});
  /// <summary>
  /// Gets the value of the open faq label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string OpenFAQLabel => Loc.Localize("OpenFaqLabel", "Open FAQ");
#endregion
#region Join Discord Button
  /// <summary>
  /// The hidden
  /// </summary>
  private Visibility _discordVisibility = Visibility.Hidden;
  /// <summary>
  /// Gets or sets the value of the discord visibility
  /// </summary>
  public Visibility DiscordVisibility {
    get => _discordVisibility;
    set => this.SetProperty(ref _discordVisibility, value);
  }
  /// <summary>
  /// Gets the value of the join discord button command
  /// </summary>
  public ICommand JoinDiscordButtonCommand => new CommandImpl(() => {});
  /// <summary>
  /// Gets the value of the join discord button label
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  public string JoinDiscordButtonLabel => Loc.Localize(nameof(JoinDiscordButtonLabel), "Join Discord");
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
  /// Gets the value of the copy message text label
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