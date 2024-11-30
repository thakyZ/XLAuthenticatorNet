using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using Serilog;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Models.ViewModel;

namespace XLAuthenticatorNet.Windows;

/// <summary>
/// The custom message box class
/// </summary>
/// <seealso cref="Window"/>
public partial class CustomMessageBox : Window {
  /// <summary>
  /// The result
  /// </summary>
  private MessageBoxResult _result;
  /// <summary>
  /// Gets the value of the view model
  /// </summary>
  private CustomMessageBoxViewModel ViewModel => (DataContext as CustomMessageBoxViewModel)!;
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
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  /// <exception cref="ArgumentOutOfRangeException">null</exception>
  [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly")]
  private CustomMessageBox(Builder builder) {
    _result = builder.CancelResult;
    InitializeComponent();
    this.DataContext = new CustomMessageBoxViewModel(this);
    if (builder.ParentWindow?.IsVisible ?? false) {
      Owner = builder.ParentWindow;
      ShowInTaskbar = false;
    }
    else {
      ShowInTaskbar = true;
    }

    ViewModel.Caption = builder.Caption;
    ViewModel.Message = builder.Text;
    if (string.IsNullOrWhiteSpace(builder.Description)) {
      ViewModel.DescriptionVisibility = Visibility.Collapsed;
    } else {
      this.Description.Document.Blocks.Clear();
      this.Description.Document.Blocks.Add(new Paragraph(new Run(builder.Description)));
    }

    switch (builder.Buttons) {
      // ReSharper disable SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
      case MessageBoxButton.OK:
        ViewModel.OKVisibility = Visibility.Visible;
        ViewModel.YesVisibility = Visibility.Collapsed;
        ViewModel.CancelVisibility = Visibility.Collapsed;
        ViewModel.NoVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.OK => OKButton,
          _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult), builder.DefaultResult, null),
        }).Focus();
        break;
      case MessageBoxButton.OKCancel:
        ViewModel.OKVisibility = Visibility.Visible;
        ViewModel.CancelVisibility = Visibility.Visible;
        ViewModel.YesVisibility = Visibility.Collapsed;
        ViewModel.NoVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.OK => OKButton,
          MessageBoxResult.Cancel => CancelButton,
          _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult), builder.DefaultResult, null),
        }).Focus();
        break;
      case MessageBoxButton.YesNoCancel:
        ViewModel.YesVisibility = Visibility.Visible;
        ViewModel.NoVisibility = Visibility.Visible;
        ViewModel.CancelVisibility = Visibility.Visible;
        ViewModel.OKVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.Yes => YesButton,
          MessageBoxResult.No => NoButton,
          MessageBoxResult.Cancel => CancelButton,
          _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult), builder.DefaultResult, null),
        }).Focus();
        break;
      case MessageBoxButton.YesNo:
        ViewModel.YesVisibility = Visibility.Visible;
        ViewModel.NoVisibility = Visibility.Visible;
        ViewModel.OKVisibility = Visibility.Collapsed;
        ViewModel.CancelVisibility = Visibility.Collapsed;
        (builder.DefaultResult switch {
          MessageBoxResult.Yes => YesButton,
          MessageBoxResult.No => NoButton,
          _ => throw new ArgumentOutOfRangeException(nameof(builder.DefaultResult), builder.DefaultResult, null),
        }).Focus();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(builder.Buttons), builder.Buttons, null);
      // ReSharper restore SwitchExpressionHandlesSomeKnownEnumValuesWithExceptionInDefault
    }

    switch (builder.Image) {
      case MessageBoxImage.None:
        ViewModel.IconVisibility = Visibility.Collapsed;
        break;
      case MessageBoxImage.Hand:
        ViewModel.IconVisibility = Visibility.Visible;
        ViewModel.IconKind = PackIconKind.Error;
        ViewModel.IconColor = Brushes.Red;
        SystemSounds.Hand.Play();
        break;
      case MessageBoxImage.Question:
        ViewModel.IconVisibility = Visibility.Visible;
        ViewModel.IconKind = PackIconKind.QuestionMarkCircle;
        ViewModel.IconColor = Brushes.DodgerBlue;
        SystemSounds.Question.Play();
        break;
      case MessageBoxImage.Exclamation:
        ViewModel.IconVisibility = Visibility.Visible;
        ViewModel.IconKind = PackIconKind.Warning;
        ViewModel.IconColor = Brushes.Yellow;
        SystemSounds.Exclamation.Play();
        break;
      case MessageBoxImage.Asterisk:
        ViewModel.IconVisibility = Visibility.Visible;
        ViewModel.IconKind = PackIconKind.Information;
        ViewModel.IconColor = Brushes.DodgerBlue;
        SystemSounds.Asterisk.Play();
        break;
      default:
        throw new ArgumentOutOfRangeException(nameof(builder.Image), builder.Image, null);
    }

    ViewModel.DiscordVisibility = builder.ShowDiscordLink ? Visibility.Visible : Visibility.Collapsed;
    ViewModel.FAQVisibility = builder.ShowHelpLinks ? Visibility.Visible : Visibility.Collapsed;
    ViewModel.ReportIssueVisibility = builder.ShowNewGitHubIssue ? Visibility.Visible : Visibility.Collapsed;
    Topmost = builder.OverrideTopMostFromParentWindow
      ? builder.ParentWindow?.Topmost ?? builder.TopMost
      : builder.TopMost;
  }

  /// <summary>
  /// Sets the result using the specified result
  /// </summary>
  /// <param name="result">The result</param>
  internal void SetResult(MessageBoxResult result) {
    _result = result;
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
  internal class Builder {
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
      Text = text;
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
      Text = string.Format(format, args);
      return this;
    }

    /// <summary>
    /// Adds the append text using the specified text
    /// </summary>
    /// <param name="text">The text</param>
    /// <returns>The builder</returns>
    internal Builder WithAppendText(string? text) {
      Text = (Text ?? string.Empty) + text;
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
      Text = (Text ?? string.Empty) + string.Format(format, args);
      return this;
    }

    /// <summary>
    /// Adds the caption using the specified caption
    /// </summary>
    /// <param name="caption">The caption</param>
    /// <returns>The builder</returns>
    internal Builder WithCaption(string caption) {
      Caption = caption;
      return this;
    }

    /// <summary>
    /// Adds the description using the specified description
    /// </summary>
    /// <param name="description">The description</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithDescription(string description) {
      Description = description;
      return this;
    }

    /// <summary>
    /// Adds the append description using the specified description
    /// </summary>
    /// <param name="description">The description</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithAppendDescription(string description) {
      Description = (Description ?? "") + description;
      return this;
    }

    /// <summary>
    /// Adds the buttons using the specified buttons
    /// </summary>
    /// <param name="buttons">The buttons</param>
    /// <returns>The builder</returns>
    internal Builder WithButtons(MessageBoxButton buttons) {
      Buttons = buttons;
      return this;
    }

    /// <summary>
    /// Adds the default result using the specified result
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithDefaultResult(MessageBoxResult result) {
      DefaultResult = result;
      return this;
    }

    /// <summary>
    /// Adds the cancel result using the specified result
    /// </summary>
    /// <param name="result">The result</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithCancelResult(MessageBoxResult result) {
      CancelResult = result;
      return this;
    }

    /// <summary>
    /// Adds the image using the specified image
    /// </summary>
    /// <param name="image">The image</param>
    /// <returns>The builder</returns>
    internal Builder WithImage(MessageBoxImage image) {
      Image = image;
      return this;
    }

    /// <summary>
    /// Adds the top most using the specified top most
    /// </summary>
    /// <param name="topMost">The top most</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    internal Builder WithTopMost(bool topMost = true) {
      TopMost = topMost;
      return this;
    }

    /// <summary>
    /// Adds the exit on close using the specified exit on close mode
    /// </summary>
    /// <param name="exitOnCloseMode">The exit on close mode</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithExitOnClose(ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.ExitOnClose) {
      ExitOnCloseMode = exitOnCloseMode;
      return this;
    }

    /// <summary>
    /// Adds the show help links using the specified show help links
    /// </summary>
    /// <param name="showHelpLinks">The show help links</param>
    /// <returns>The builder</returns>
    internal Builder WithShowHelpLinks(bool showHelpLinks = true) {
      ShowHelpLinks = showHelpLinks;
      return this;
    }

    /// <summary>
    /// Adds the show discord link using the specified show discord link
    /// </summary>
    /// <param name="showDiscordLink">The show discord link</param>
    /// <returns>The builder</returns>
    internal Builder WithShowDiscordLink(bool showDiscordLink = true) {
      ShowDiscordLink = showDiscordLink;
      return this;
    }

    /// <summary>
    /// Adds the show new git hub issue using the specified show new git hub issue
    /// </summary>
    /// <param name="showNewGitHubIssue">The show new git hub issue</param>
    /// <returns>The builder</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal Builder WithShowNewGitHubIssue(bool showNewGitHubIssue = true) {
      ShowNewGitHubIssue = showNewGitHubIssue;
      return this;
    }

    /// <summary>
    /// Adds the parent window using the specified window
    /// </summary>
    /// <param name="window">The window</param>
    /// <returns>The builder</returns>
    internal Builder WithParentWindow(Window? window) {
      ParentWindow = window;
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
      ParentWindow = window;
      OverrideTopMostFromParentWindow = overrideTopMost;
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
      this.WithAppendDescription("\n\nVersion: " + Util.GetAssemblyVersion())
        .WithAppendDescription("\nGit Hash: " + Util.GetGitHash()).WithAppendDescription("\nContext: " + context)
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
      Builder builder = new Builder().WithText(_errorExplanation).WithExitOnClose(exitOnCloseMode)
        .WithImage(MessageBoxImage.Error).WithShowHelpLinks().WithShowDiscordLink().WithShowNewGitHubIssue()
        .WithAppendDescription(exc.ToString()).WithAppendSettingsDescription(context);
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
    internal static Builder NewFromUnexpectedException(Exception exc, string context, ExitOnCloseModes exitOnCloseMode = ExitOnCloseModes.DontExitOnClose) =>
      NewFrom(exc, context, exitOnCloseMode)
        .WithAppendTextFormatted(Loc.Localize("UnexpectedErrorSummary", "Unexpected error has occurred. ({0})"),
          exc.Message).WithAppendText("\n")
        .WithAppendText(Loc.Localize("UnexpectedErrorActionable", "Please report this error."));

    /// <summary>
    /// Shows the assuming dispatcher thread
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    /// <exception cref="NotImplementedException"></exception>
    /// <returns>The message box result</returns>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    internal MessageBoxResult ShowAssumingDispatcherThread() {
      DefaultResult = DefaultResult != MessageBoxResult.None
        ? DefaultResult
        : Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.OK,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Yes,
          MessageBoxButton.YesNo => MessageBoxResult.Yes,
          _ => throw new NotImplementedException(),
        };
      CancelResult = CancelResult != MessageBoxResult.None
        ? CancelResult
        : Buttons switch {
          MessageBoxButton.OK => MessageBoxResult.OK,
          MessageBoxButton.OKCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNoCancel => MessageBoxResult.Cancel,
          MessageBoxButton.YesNo => MessageBoxResult.No,
          _ => throw new NotImplementedException(),
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
      MessageBoxResult? res = null;
      var newWindowThread = new Thread(() => res = ShowAssumingDispatcherThread());
      newWindowThread.SetApartmentState(ApartmentState.STA);
      newWindowThread.IsBackground = true;
      newWindowThread.Start();
      newWindowThread.Join();
      return res.GetValueOrDefault(CancelResult);
    }

    /// <summary>
    /// Shows this instance
    /// </summary>
    /// <returns>The result</returns>
    internal MessageBoxResult Show() {
      MessageBoxResult result;
      if (ParentWindow is not null) {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (System.Windows.Threading.Dispatcher.CurrentDispatcher == ParentWindow.Dispatcher) {
          result = ShowAssumingDispatcherThread();
        } else {
          result = ParentWindow.Dispatcher.Invoke(ShowAssumingDispatcherThread);
        }
      }
      else {
        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
          result = ShowAssumingDispatcherThread();
        } else {
          result = Application.Current.Dispatcher.Invoke(ShowAssumingDispatcherThread);
        }
      }

      // ReSharper disable once InvertIf
      if (ExitOnCloseMode == ExitOnCloseModes.ExitOnClose) {
        Log.CloseAndFlush();
        if (result == MessageBoxResult.Yes && Process.GetCurrentProcess().MainModule is {} mainModule) {
          Process.Start(mainModule.FileName, string.Join(" ", Environment.GetCommandLineArgs().Skip(1).Select(x => EncodeParameterArgument(x))));
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
  internal static MessageBoxResult Show(string text, string caption, MessageBoxButton buttons = MessageBoxButton.OK,
    MessageBoxImage image = MessageBoxImage.Asterisk, bool showHelpLinks = true, bool showDiscordLink = true, Window? parentWindow = null) {
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
    if (condition) return false;
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
    if (!force && argument.Length > 0 && argument.IndexOfAny(" \t\n\v\"".ToCharArray()) == -1) return argument;
    var quoted = new StringBuilder(argument.Length * 2);
    quoted.Append('"');
    var numberBackslashes = 0;
    foreach (char chr in argument) {
      switch (chr) {
        case '\\':
          numberBackslashes++;
          continue;
        case '"':
          quoted.Append('\\', numberBackslashes * 2 + 1);
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