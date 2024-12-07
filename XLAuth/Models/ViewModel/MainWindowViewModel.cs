using System.Windows.Input;
using CheapLoc;
using MaterialDesignThemes.Wpf.Transitions;
using XLAuth.Dialogs;
using XLAuth.Domain.Components.Helpers;
using XLAuth.Models.Abstracts;
using XLAuth.Windows;
using FadeWipe = XLAuth.Domain.Animations.FadeWipe;
using NavigationCommands = XLAuth.Domain.Components.Helpers.NavigationCommands;

namespace XLAuth.Models.ViewModel;

/// <summary>
/// The main window view model class
/// </summary>
/// <seealso cref="ViewModelBase{MainWindow}"/>
/// <seealso cref="ISlideNavigationSubject"/>
internal sealed class MainWindowViewModel : ViewModelBase<MainWindow>, ISlideNavigationSubject {
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(MainWindow) + nameof(this.Title), "XIVLauncher Authenticator");

  /// <summary>
  /// The slide navigator
  /// </summary>
  private readonly SlideNavigator _slideNavigator;

  /// <summary>
  /// The active slide index
  /// </summary>
  private int _activeSlideIndex;

  /// <summary>
  /// Gets or sets the value of the active slide index
  /// </summary>
  public int ActiveSlideIndex {
    get => this._activeSlideIndex;
    set => this.SetProperty(ref this._activeSlideIndex, value);
  }

  /// <summary>
  /// Gets the value of the slides
  /// </summary>
  public IReadOnlyList<TransitionerSlide> Slides { get; }

  /// <summary>
  /// Initializes a new instance of the <see cref="MainWindowViewModel"/> class
  /// </summary>
  /// <param name="parent">The parent</param>
  internal MainWindowViewModel(MainWindow parent) : base(parent) {
    CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(NavigationCommands.ShowSettingsCommand,                this.ShowSettingsExecuted));
    CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(NavigationCommands.HideSettingsCommand,                this.HideSettingsExecuted));
    CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(NavigationCommands.GoBackCommand,                      this.GoBackExecuted));
    CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(System.Windows.Input.NavigationCommands.BrowseBack,    this.GoBackExecuted));
    CommandManager.RegisterClassCommandBinding(typeof(MainWindow), new CommandBinding(System.Windows.Input.NavigationCommands.BrowseForward, this.GoForwardExecuted));
    this.Slides = [
      new TransitionerSlide {
        Content = parent.SettingsControl,
        ForwardWipe = new FadeWipe(),
      },
      new TransitionerSlide {
        Content = parent.MainContent,
        BackwardWipe = new FadeWipe(),
      },
    ];
    this._slideNavigator = new SlideNavigator(this, [..this.Slides]);
    this._slideNavigator.GoTo(1);
  }

#if DEBUG
  public MainWindowViewModel() {
    this.Slides = [];
    this._slideNavigator = new SlideNavigator(this, [..this.Slides]);
  }
#endif

  /// <summary>
  /// Shows the settings executed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void ShowSettingsExecuted(object? sender, ExecutedRoutedEventArgs e) {
    this._slideNavigator.GoTo<SettingsControl>();
  }

  /// <summary>
  /// Hides the settings executed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void HideSettingsExecuted(object? sender, ExecutedRoutedEventArgs e) {
    this._slideNavigator.GoTo<MainControl>();
  }

  /// <summary>
  /// Goes the back executed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void GoBackExecuted(object? sender, ExecutedRoutedEventArgs e) {
    this._slideNavigator.GoBack();
  }

  /// <summary>
  /// Goes the forward executed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void GoForwardExecuted(object? sender, ExecutedRoutedEventArgs e) {
    this._slideNavigator.GoForward();
  }
}
