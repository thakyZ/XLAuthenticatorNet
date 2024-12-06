using System.Buffers;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using XLAuth.Extensions;
using XLAuth.Models.ViewModel;
using Process = System.Diagnostics.Process;

namespace XLAuth.Windows;

/// <summary>
/// The custom message box class
/// </summary>
/// <seealso cref="Window"/>
public sealed partial class CustomMessageBox : Window {
  /// <summary>
  /// The result
  /// </summary>
  private MessageBoxResult _result;
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private CustomMessageBoxViewModel ViewModel => (this.DataContext as CustomMessageBoxViewModel)!;
  /// <summary>
  /// The localize
  /// </summary>
  private static readonly string _errorExplanation = Loc.Localize("ErrorExplanation",
    """
    An error in XIVLauncher occurred. Please consult the FAQ. If this issue persists, please report
    it on GitHub by clicking the button below, describing the issue and copying the text in the box.
    """);

  /// <summary>
  /// Initializes a new instance of the <see cref="CustomMessageBox"/> class
  /// </summary>
  /// <param name="builder">The builder</param>
  /// <exception cref="ArgumentOutOfRangeException">Thrown if </exception>
  [SuppressMessage("Usage",  "CA2208:Instantiate argument exceptions correctly"),
   SuppressMessage("Design", "MA0051:Method is too long", Justification = "This is an initializer for a new window so whatever if it's too long.")]
  private CustomMessageBox(Builder builder) {
    this._result = builder.CancelResult;
    this.InitializeComponent();
    this.DataContext = new CustomMessageBoxViewModel(this);
    if (builder.ParentWindow?.IsVisible ?? false) {
      this.Owner = builder.ParentWindow;
      this.ShowInTaskbar = false;
    }
    else {
      this.ShowInTaskbar = true;
    }

    this.ViewModel.Caption = builder.Caption;
    this.ViewModel.Message = builder.Text;
    if (string.IsNullOrWhiteSpace(builder.Description)) {
      this.ViewModel.DescriptionVisibility = Visibility.Collapsed;
    } else {
      this.Description.Document.Blocks.Clear();
      var paragraphRun = new Run(builder.Description);
      var paragraph = new Paragraph(paragraphRun);
      this.Description.Document.Blocks.Add(paragraph);
    }

    switch (builder.Buttons) {
      case MessageBoxButton.OK:
        this.ViewModel.OKButtonVisibility = Visibility.Visible;
        this.ViewModel.YesButtonVisibility = Visibility.Collapsed;
        this.ViewModel.CancelButtonVisibility = Visibility.Collapsed;
        this.ViewModel.NoVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.OK => this.OKButton,
          _ => ExceptionExtensions.ThrowEnumOutOfRangeException<Button, MessageBoxResult>(nameof(builder.DefaultResult), builder.DefaultResult),
        }).Focus();
        break;
      case MessageBoxButton.OKCancel:
        this.ViewModel.OKButtonVisibility = Visibility.Visible;
        this.ViewModel.CancelButtonVisibility = Visibility.Visible;
        this.ViewModel.YesButtonVisibility = Visibility.Collapsed;
        this.ViewModel.NoVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.OK => this.OKButton,
          MessageBoxResult.Cancel => this.CancelButton,
          _ => ExceptionExtensions.ThrowEnumOutOfRangeException<Button, MessageBoxResult>(nameof(builder.DefaultResult), builder.DefaultResult),
        }).Focus();
        break;
      case MessageBoxButton.YesNoCancel:
        this.ViewModel.YesButtonVisibility = Visibility.Visible;
        this.ViewModel.NoVisibility = Visibility.Visible;
        this.ViewModel.CancelButtonVisibility = Visibility.Visible;
        this.ViewModel.OKButtonVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.Yes => this.YesButton,
          MessageBoxResult.No => this.NoButton,
          MessageBoxResult.Cancel => this.CancelButton,
          _ => ExceptionExtensions.ThrowEnumOutOfRangeException<Button, MessageBoxResult>(nameof(builder.DefaultResult), builder.DefaultResult),
        }).Focus();
        break;
      case MessageBoxButton.YesNo:
        this.ViewModel.YesButtonVisibility = Visibility.Visible;
        this.ViewModel.NoVisibility = Visibility.Visible;
        this.ViewModel.OKButtonVisibility = Visibility.Collapsed;
        this.ViewModel.CancelButtonVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.Yes => this.YesButton,
          MessageBoxResult.No => this.NoButton,
          _ => ExceptionExtensions.ThrowEnumOutOfRangeException<System.Windows.Controls.Button, MessageBoxResult>(nameof(builder.DefaultResult), builder.DefaultResult),
        }).Focus();
        break;
      default:
        ExceptionExtensions.ThrowEnumOutOfRangeException<System.Windows.Controls.Button, MessageBoxResult>(nameof(builder.DefaultResult), builder.DefaultResult);
        return;
    }

    switch (builder.Image) {
      case MessageBoxImage.None:
        this.ViewModel.IconVisibility = Visibility.Collapsed;
        break;
      case MessageBoxImage.Hand:
        this.ViewModel.IconVisibility = Visibility.Visible;
        this.ViewModel.IconKind = PackIconKind.Error;
        this.ViewModel.IconColor = Brushes.Red;
        SystemSounds.Hand.Play();
        break;
      case MessageBoxImage.Question:
        this.ViewModel.IconVisibility = Visibility.Visible;
        this.ViewModel.IconKind = PackIconKind.QuestionMarkCircle;
        this.ViewModel.IconColor = Brushes.DodgerBlue;
        SystemSounds.Question.Play();
        break;
      case MessageBoxImage.Exclamation:
        this.ViewModel.IconVisibility = Visibility.Visible;
        this.ViewModel.IconKind = PackIconKind.Warning;
        this.ViewModel.IconColor = Brushes.Yellow;
        SystemSounds.Exclamation.Play();
        break;
      case MessageBoxImage.Asterisk:
        this.ViewModel.IconVisibility = Visibility.Visible;
        this.ViewModel.IconKind = PackIconKind.Information;
        this.ViewModel.IconColor = Brushes.DodgerBlue;
        SystemSounds.Asterisk.Play();
        break;
      default:
        ExceptionExtensions.ThrowEnumOutOfRangeException<object, MessageBoxImage>(nameof(builder.Image), builder.Image);
        break;
    }

    this.ViewModel.DiscordVisibility = builder.ShowDiscordLink ? Visibility.Visible : Visibility.Collapsed;
    this.ViewModel.FAQVisibility = builder.ShowHelpLinks ? Visibility.Visible : Visibility.Collapsed;
    this.ViewModel.ReportIssueVisibility = builder.ShowNewGitHubIssue ? Visibility.Visible : Visibility.Collapsed;

    this.Topmost = builder.OverrideTopMostFromParentWindow
      ? builder.ParentWindow?.Topmost ?? builder.TopMost
      : builder.TopMost;
  }

  /// <summary>
  /// Sets the result using the specified result
  /// </summary>
  /// <param name="result">The result</param>
  internal void SetResult(MessageBoxResult result) {
    this._result = result;
    this.Close();
  }

  /// <summary>
  /// The exit on close modes enum
  /// </summary>
  internal enum ExitOnCloseModes {
    /// <summary>
    /// The dont exit on close exit on close modes
    /// </summary>
    DontExitOnClose,
    /// <summary>
    /// The exit on close exit on close modes
    /// </summary>
    ExitOnClose,
  }

  /// <summary>
  /// The builder class
  /// </summary>
  internal sealed class Builder {
    /// <summary>
    /// The text
    /// </summary>
    internal string? Text { get; private set; }
    /// <summary>
    /// The caption
    /// </summary>
    internal string Caption { get; private set; } = "XIVLauncher";
    /// <summary>
    /// The description
    /// </summary>
    internal string? Description { get; private set; }
    /// <summary>
    /// The ok
    /// </summary>
    internal MessageBoxButton Buttons { get; private set; } = MessageBoxButton.OK;
    /// <summary>
    /// The none
    /// </summary>
    internal MessageBoxResult DefaultResult { get; private set; } = MessageBoxResult.None; // On enter
    /// <summary>
    /// The none
    /// </summary>
    internal MessageBoxResult CancelResult { get; private set; } = MessageBoxResult.None; // On escape
    /// <summary>
    /// The none
    /// </summary>
    internal MessageBoxImage Image { get; private set; } = MessageBoxImage.None;
    /// <summary>
    /// The restart button
    /// </summary>
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    internal bool RestartButton { get; private set; }
    /// <summary>
    /// The exit button
    /// </summary>
    [SuppressMessage("ReSharper", "NotAccessedField.Global")]
    internal bool ExitButton { get; private set; }
    /// <summary>
    /// The top most
    /// </summary>
    internal bool TopMost { get; private set; }
    /// <summary>
    /// The dont exit on close
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal ExitOnCloseModes ExitOnCloseMode { get; private set; } = ExitOnCloseModes.DontExitOnClose;
    /// <summary>
    /// The show help links
    /// </summary>
    internal bool ShowHelpLinks { get; private set; }
    /// <summary>
    /// The show discord link
    /// </summary>
    internal bool ShowDiscordLink { get; private set; }
    /// <summary>
    /// The show a button new GitHub issue
    /// </summary>
    internal bool ShowNewGitHubIssue { get; private set; }
    /// <summary>
    /// The parent window
    /// </summary>
    internal Window? ParentWindow { get; private set; }
    /// <summary>
    /// The override top most from parent window
    /// </summary>
    internal bool OverrideTopMostFromParentWindow { get; private set; } = true;

    /// <summary>
    /// Initializes a new instance of the <see cref="Builder"/> class
    /// </summary>
    [SuppressMessage("ReSharper", "EmptyConstructor")]
    internal Builder() { }

    /// <summary>
    /// Adds the text using the specified text
    /// </summary>
    /// <param name="text">The text</param>
    /// <returns>The builder</returns>
    internal Builder WithText(string text) {
      this.Text = text;
      return this;
    }

    /// <summary>
    /// Adds the text formatted using the specified format
    /// </summary>
    /// <param name="format">The format</param>
    /// <param name="args">The args</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithTextFormatted(string format, params object[] args) {
      this.Text = string.Format(format, args);
      return this;
    }

    /// <summary>
    /// Adds the append text using the specified text
    /// </summary>
    /// <param name="text">The text</param>
    /// <returns>The builder</returns>
    internal Builder WithAppendText(string? text) {
      this.Text = (this.Text ?? string.Empty) + text;
      return this;
    }

    /// <summary>
    /// Adds the append text formatted using the specified format
    /// </summary>
    /// <param name="format">The format</param>
    /// <param name="args">The args</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithAppendTextFormatted(string format, params object[] args) {
      this.Text = (this.Text ?? string.Empty) + string.Format(format, args);
      return this;
    }

    /// <summary>
    /// Adds the caption using the specified caption
    /// </summary>
    /// <param name="caption">The caption</param>
    /// <returns>The builder</returns>
    internal Builder WithCaption(string caption) {
      this.Caption = caption;
      return this;
    }

    /// <summary>
    /// Adds the description using the specified description
    /// </summary>
    /// <param name="description">The description</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithDescription(string description) {
      this.Description = description;
      return this;
    }

    /// <summary>
    /// Adds the append description using the specified description
    /// </summary>
    /// <param name="description">The description</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithAppendDescription(string description) {
      this.Description = (this.Description ?? "") + description;
      return this;
    }

    /// <summary>
    /// Adds the buttons using the specified buttons
    /// </summary>
    /// <param name="buttons">The buttons</param>
    /// <returns>The builder</returns>
    internal Builder WithButtons(MessageBoxButton buttons) {
      this.Buttons = buttons;
      return this;
    }

    /// <summary>
    /// Adds the default result using the specified result
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithDefaultResult(MessageBoxResult result) {
      this.DefaultResult = result;
      return this;
    }

    /// <summary>
    /// Adds the cancel result using the specified result
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithCancelResult(MessageBoxResult result) {
      this.CancelResult = result;
      return this;
    }

    /// <summary>
    /// Adds the image using the specified image
    /// </summary>
    /// <param name="image">The image</param>
    /// <returns>The builder</returns>
    internal Builder WithImage(MessageBoxImage image) {
      this.Image = image;
      return this;
    }

    /// <summary>
    /// Adds the top most using the specified top most
    /// </summary>
    /// <param name="topMost">The top most</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithTopMost(bool topMost = true) {
      this.TopMost = topMost;
      return this;
    }

    /// <summary>
    /// Adds the exit on close using the specified exit on close mode
    /// </summary>
    /// <param name="exitOnCloseMode">The exit on close mode</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithExitOnClose(ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.ExitOnClose) {
      this.ExitOnCloseMode = exitOnCloseMode;
      return this;
    }

    /// <summary>
    /// Adds the show help links using the specified show help links
    /// </summary>
    /// <param name="showHelpLinks">The show help links</param>
    /// <returns>The builder</returns>
    internal Builder WithShowHelpLinks(bool showHelpLinks = true) {
      this.ShowHelpLinks = showHelpLinks;
      return this;
    }

    /// <summary>
    /// Adds the show discord link using the specified show discord link
    /// </summary>
    /// <param name="showDiscordLink">The show discord link</param>
    /// <returns>The builder</returns>
    internal Builder WithShowDiscordLink(bool showDiscordLink = true) {
      this.ShowDiscordLink = showDiscordLink;
      return this;
    }

    /// <summary>
    /// Adds the show new git hub issue using the specified show new git hub issue
    /// </summary>
    /// <param name="showNewGitHubIssue">The show new git hub issue</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithShowNewGitHubIssue(bool showNewGitHubIssue = true) {
      this.ShowNewGitHubIssue = showNewGitHubIssue;
      return this;
    }

    /// <summary>
    /// Adds the parent window using the specified window
    /// </summary>
    /// <param name="window">The window</param>
    /// <returns>The builder</returns>
    internal Builder WithParentWindow(Window? window) {
      this.ParentWindow = window;
      return this;
    }

    /// <summary>
    /// Adds the parent window using the specified window
    /// </summary>
    /// <param name="window">The window</param>
    /// <param name="overrideTopMost">The override top most</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithParentWindow(Window? window, bool overrideTopMost) {
      this.ParentWindow = window;
      this.OverrideTopMostFromParentWindow = overrideTopMost;
      return this;
    }

    /// <summary>
    /// Adds the restart button
    /// </summary>
    /// <returns>The builder</returns>
    private Builder WithRestartButton() {
      this.RestartButton = true;
      return this;
    }

    /// <summary>
    /// Adds the exit button
    /// </summary>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Local")]
    private Builder WithExitButton() {
      this.ExitButton = true;
      return this;
    }

    /// <summary>
    /// Adds the exception text
    /// </summary>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithExceptionText() =>
      this.WithText(Loc.Localize("ErrorExplanation",
        """
        An error in XIVLauncher occurred. Please consult the FAQ. If this issue persists, please report
        it on GitHub by clicking the button below, describing the issue and copying the text in the box.
        """));

    /// <summary>
    /// Adds the append settings description using the specified context
    /// </summary>
    /// <param name="context">The context</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithAppendSettingsDescription(string context) {
      this.WithAppendDescription("\n\nVersion: " + XLAuth.Support.Util.GetAssemblyVersion())
        .WithAppendDescription("\nGit Hash: " + XLAuth.Support.Util.GetGitHash()).WithAppendDescription("\nContext: " + context)
        .WithAppendDescription("\nOS: " + Environment.OSVersion)
        .WithAppendDescription("\n64bit? " + Environment.Is64BitProcess)
        .WithAppendDescription("\nLanguage: " + App.Settings.Language);
#if DEBUG
      this.WithAppendDescription("\nDebugging");
#endif
      return this;
    }

    /// <summary>
    /// News the from using the specified text
    /// </summary>
    /// <param name="text">The text</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal static Builder NewFrom(string text) => new Builder().WithText(text);

    /// <summary>
    /// News the from using the specified exc
    /// </summary>
    /// <param name="exc">The exc</param>
    /// <param name="context">The context</param>
    /// <param name="exitOnCloseMode">The exit on close mode</param>
    /// <returns>The builder</returns>
    internal static Builder NewFrom(Exception exc, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
      var exceptionText = exc.ToString();
      Builder builder = new Builder().WithText(_errorExplanation).WithExitOnClose(exitOnCloseMode)
                                     .WithImage(MessageBoxImage.Error).WithShowHelpLinks().WithShowDiscordLink().WithShowNewGitHubIssue()
                                     .WithAppendDescription(exceptionText).WithAppendSettingsDescription(context);
      if (exitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
        builder.WithButtons(MessageBoxButton.YesNo).WithRestartButton().WithExitButton();
      }
      return builder;
    }

    /// <summary>
    /// News the from unexpected exception using the specified exc
    /// </summary>
    /// <param name="exc">The exc</param>
    /// <param name="context">The context</param>
    /// <param name="exitOnCloseMode">The exit on close mode</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal static Builder NewFromUnexpectedException(Exception exc, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) {
      var unexpectedErrorSummary = Loc.Localize("UnexpectedErrorSummary", "Unexpected error has occurred. ({0})");
      var unexpectedErrorActionable = Loc.Localize("UnexpectedErrorActionable", "Please report this error.");
      return Builder.NewFrom(exc, context, exitOnCloseMode)
             .WithAppendTextFormatted(unexpectedErrorSummary, exc.Message).WithAppendText("\n")
             .WithAppendText(unexpectedErrorActionable);
    }

    /// <summary>
    /// Shows the assuming dispatcher thread
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    /// <returns>The message box result</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal MessageBoxResult ShowAssumingDispatcherThread() {
      this.DefaultResult = this.DefaultResult != MessageBoxResult.None
        ? this.DefaultResult
        : this.Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.OK,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
          MessageBoxButton.YesNo => MessageBoxResult.Yes,
          _ => throw new NotSupportedException(),
        };

      this.CancelResult = this.CancelResult != MessageBoxResult.None
        ? this.CancelResult
        : this.Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNo => MessageBoxResult.No,
          _ => throw new NotSupportedException(),
        };
      var res = new CustomMessageBox(this);
      res.ShowDialog();
      return res._result;
    }

    /// <summary>
    /// Shows the in new thread
    /// </summary>
    /// <returns>The message box result</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal MessageBoxResult ShowInNewThread() {
      MessageBoxResult? result = null;
      var newWindowThread = new Thread(() => result = this.ShowAssumingDispatcherThread());
      newWindowThread.SetApartmentState(ApartmentState.STA);
      newWindowThread.IsBackground = true;
      newWindowThread.Start();
      newWindowThread.Join();
      return result ?? this.CancelResult;
    }

    /// <summary>
    /// Shows this instance
    /// </summary>
    /// <returns>The result</returns>
    internal MessageBoxResult Show() {
      MessageBoxResult result;
      if (this.ParentWindow is not null) {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (System.Windows.Threading.Dispatcher.CurrentDispatcher == this.ParentWindow.Dispatcher) {
          result = this.ShowAssumingDispatcherThread();
        } else {
          result = this.ParentWindow.Dispatcher.Invoke(this.ShowAssumingDispatcherThread);
        }
      }
      else {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
          result = this.ShowAssumingDispatcherThread();
        } else {
          result = Application.Current.Dispatcher.Invoke(this.ShowAssumingDispatcherThread);
        }
      }

      // ReSharper disable once InvertIf
      if (this.ExitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
        Serilog.Log.CloseAndFlush();
        if (result == MessageBoxResult.Yes && Process.GetCurrentProcess().MainModule is {} mainModule) {
          Process.Start(mainModule.FileName, string.Join(' ', Environment.GetCommandLineArgs().Skip(1).Select(item => EncodeParameterArgument(item))));
        }

        Environment.Exit(-1);
      }

      return result;
    }
  }

  /// <summary>
  /// Shows the text
  /// </summary>
  /// <param name="text">The text</param>
  /// <param name="caption">The caption</param>
  /// <param name="buttons">The buttons</param>
  /// <param name="image">The image</param>
  /// <param name="showHelpLinks">The show help links</param>
  /// <param name="showDiscordLink">The show discord link</param>
  /// <param name="parentWindow">The parent window</param>
  /// <returns>The message box result</returns>
  [SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
  internal static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Asterisk, bool showHelpLinks = true, bool showDiscordLink = true, Window? parentWindow = null) {
    return new Builder().WithCaption(caption).WithText(text).WithButtons(buttons).WithImage(image)
      .WithShowHelpLinks(showHelpLinks).WithShowDiscordLink(showDiscordLink).WithParentWindow(parentWindow).Show();
  }

  /// <summary>
  /// Asserts the or show error using the specified condition
  /// </summary>
  /// <param name="condition">The condition</param>
  /// <param name="context">The context</param>
  /// <param name="fatal">The fatal</param>
  /// <param name="parentWindow">The parent window</param>
  /// <exception cref="InvalidOperationException">Assertion failure.</exception>
  /// <returns>The bool</returns>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  internal static bool AssertOrShowError(bool condition, string context, bool fatal = false, Window? parentWindow = null) {
    if (condition) {
      return false;
    }

    try {
      throw new InvalidOperationException("Assertion failure.");
    } catch (Exception e) {
      Builder.NewFrom(e, context, fatal ? ExitOnCloseModes.ExitOnClose : ExitOnCloseModes.DontExitOnClose)
        .WithAppendText("\n\n")
        .WithAppendText(Loc.Localize("ErrorAssertionFailed", "Something that cannot happen happened."))
        .WithParentWindow(parentWindow).Show();
    }

    return true;
  }

  // https://docs.microsoft.com/en-us/archive/blogs/twistylittlepassagesallalike/everyone-quotes-command-line-arguments-the-wrong-way
  /// <summary>
  /// Encodes the parameter argument using the specified argument
  /// </summary>
  /// <param name="argument">The argument</param>
  /// <param name="force">The force</param>
  /// <returns>The string</returns>
  private static string EncodeParameterArgument(string argument, bool force = false) {
    var searchChars = SearchValues.Create(" \t\n\v\"");
    if (!force && argument.Length > 0 && argument.AsSpan().IndexOfAny(searchChars) == -1) {
      return argument;
    }

    var quoted = new StringBuilder(argument.Length * 2);
    quoted.Append('"');
    var numberBackslashes = 0;
    foreach (char chr in argument) {
      switch (chr) {
        case '\\':
          numberBackslashes++;
          continue;
        case '"':
          quoted.Append('\\', (numberBackslashes * 2) + 1);
          break;
        default:
          quoted.Append('\\', numberBackslashes);
          break;
      }

      quoted.Append(chr);

      numberBackslashes = 0;
    }

    quoted.Append('\\', numberBackslashes * 2)
          .Append('"');
    return quoted.ToString();
  }
}
