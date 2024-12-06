using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CheapLoc;
using MaterialDesignThemes.Wpf;
using XLAuthenticatorNet.Config;
using XLAuthenticatorNet.Dialogs;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Domain.Commands;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Models.Abstracts;
using XLAuthenticatorNet.Models.Events;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet.Models.ViewModel;

/// <summary>
/// The settings control view model class
/// </summary>
/// <seealso cref="ViewModelBase{SettingsControl}"/>
[SuppressMessage("Performance", "CA1822:Mark members as static"),
 SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global")]
internal sealed class SettingsControlViewModel : ViewModelBase<SettingsControl> {
#region Binding Properties
#region Localization
  /// <summary>
  /// Gets the value of the title
  /// </summary>
  public string Title => Loc.Localize(nameof(SettingsControl) + nameof(this.Title), "Settings");
  /// <summary>
  /// Gets the value of the registered label
  /// </summary>
  public string RegisteredLabel => Loc.Localize(nameof(this.RegisteredLabel), "Registered: ");
  /// <summary>
  /// Gets the value of the setup OTP code button text
  /// </summary>
  public string SetupOTPCodeButtonLabel => Loc.Localize(nameof(this.SetupOTPCodeButtonLabel), "Set-up OTP Code");
  /// <summary>
  /// Gets the value of the xiv launcher IP label
  /// </summary>
  public string XIVLauncherIPLabel => Loc.Localize(nameof(this.XIVLauncherIPLabel), "XIVLauncher IP: ");
  /// <summary>
  /// Gets the value of the xiv launcher IP button text
  /// </summary>
  public string XIVLauncherIPButtonLabel => Loc.Localize(nameof(this.XIVLauncherIPButtonLabel), "Set-up XIVLauncher IP");
  /// <summary>
  /// Gets the value of the OTP label
  /// </summary>
  public string YourOTPLabel => Loc.Localize(nameof(this.YourOTPLabel), "Your OTP:");
  /// <summary>
  /// Gets the value of the OTP label
  /// </summary>
  public string SaveSettingsButtonLabel => Loc.Localize(nameof(this.SaveSettingsButtonLabel), "Save");
  /// <summary>
  /// Gets the value of the close app after sending check box label
  /// </summary>
  public string CloseAppAfterSendingCheckBoxLabel => Loc.Localize(nameof(this.CloseAppAfterSendingCheckBoxLabel), "Close App After Sending: ");
#endregion Localization

#region Config Options
  /// <summary>
  /// Gets or sets the value of the close app after sending
  /// </summary>
  public bool CloseAppAfterSending {
    get => App.Settings.CloseApp;
    set => App.Settings.CloseApp = value;
  }
#endregion Config Options

#region Diagnostic Properties
  /// <summary>
  /// The empty
  /// </summary>
  private object? _launcherIPText = string.Empty;

  /// <summary>
  /// Gets or sets the value of the launcher IP text
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public object? LauncherIPText {
    get => this._launcherIPText;
    set => this.SetProperty(ref this._launcherIPText, value);
  }

  /// <summary>
  /// The black
  /// </summary>
  private Brush _launcherIPColor = Brushes.Black;

  /// <summary>
  /// Gets or sets the value of the launcher IP color
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public Brush LauncherIPColor {
    get => this._launcherIPColor;
    set => this.SetProperty(ref this._launcherIPColor, value);
  }

  /// <summary>
  /// The empty
  /// </summary>
  private string _isRegisteredText = string.Empty;

  /// <summary>
  /// Gets or sets the value of the is registered text
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  public string IsRegisteredText {
    get => this._isRegisteredText;
    set => this.SetProperty(ref this._isRegisteredText, value);
  }
#endregion Diagnostic Properties

#region Account Selection Properties
  /// <summary>
  /// Gets the value of the current account name
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public string CurrentAccountName => App.AccountManager.CurrentAccount?.Name ?? "NULL";

  /// <summary>
  /// The black
  /// </summary>
  private object _popupContent = Brushes.Black;

  /// <summary>
  /// Gets or sets the value of the popup content
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedMember.Global")]
  public object PopupContent {
    get => this._popupContent;
    set => this.SetProperty(ref this._popupContent, value);
  }

  /// <summary>
  /// The black
  /// </summary>
  private Brush _isRegisteredColor = Brushes.Black;

  /// <summary>
  /// Gets or sets the value of the is registered color
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("ReSharper", "UnusedMember.Global")]
  public Brush IsRegisteredColor {
    get => this._isRegisteredColor;
    set => this.SetProperty(ref this._isRegisteredColor, value);
  }

  /// <summary>
  /// The account selection button content
  /// </summary>
  private object? _accountSelectionButtonContent;

  /// <summary>
  /// Gets or sets the value of the account selection button content
  /// </summary>
  public object? AccountSelectionButtonContent {
    get => this._accountSelectionButtonContent;
    set => this.SetProperty(ref this._accountSelectionButtonContent, value);
  }
#endregion Account Selection Properties

#region Language Selection Properties
  /// <summary>
  /// Gets the value of the current account name
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global")]
  public string CurrentLanguageName => App.Settings.Language.GetName() ?? "NULL";

  /// <summary>
  /// The language selection button content
  /// </summary>
  private object? _languageSelectionButtonContent;

  /// <summary>
  /// Gets or sets the value of the language selection button content
  /// </summary>
  public object? LanguageSelectionButtonContent {
    get => this._languageSelectionButtonContent;
    set => this.SetProperty(ref this._languageSelectionButtonContent, value);
  }
#endregion Language Selection Properties

#region Commands
  /// <summary>
  /// Gets the value of the Settings Back Command
  /// </summary>
  public ICommand SettingsBack => new CommandImpl(() => this.Parent.CloseAndCancelSettings());

  /// <summary>
  /// Gets the value of the Save Settings Command
  /// </summary>
  public ICommand SaveSettingsButtonCommand => new CommandImpl(() => {
    App.AccountManager.Save();
    App.Settings.Save();
  });

  /// <summary>
  /// Gets the value of the close app after sending check box command
  /// </summary>
  public ICommand CloseAppAfterSendingCheckBoxCommand => new CommandImpl(() => {
    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    // Extra null checks here so that the XAML preview can actually work.
    bool? isChecked = this.Parent?.CloseAppAfterSendingCheckBox?.IsChecked ?? false;

    if (isChecked is not null && isChecked.Value != this.CloseAppAfterSending) {
      App.Settings.CloseApp = isChecked.Value;
    }
  });

  /// <summary>
  /// Gets the value of the set OTP key dialog command
  /// </summary>
  public ICommand SetOtpKeyDialogCommand => new AsyncCommandImpl(async () => {
    var     view    = new OTPKeyDialog();
    object? @object = await DialogHost.Show(view, "OTPKeyDialogHost").ConfigureAwait(false);

    if (@object is not DialogResult<SecureString> result) {
      throw new Exception("Returned object from DialogHost is not typeof DialogResult!");
    }

    if (result.Result == MessageBoxResult.Cancel) {
      Logger.Debug("Result: {0} | Value: {1}", result.Result, result.Value);

      return;
    }

    this.SetOTPKey(!result.Value.IsNullOrEmptyOrWhiteSpace());
    App.AccountManager.UpdateCurrentAccount(result.Value);
  });

  /// <summary>
  /// Gets the value of the set launcher IP dialog command
  /// </summary>
  public ICommand SetLauncherIPDialogCommand => new AsyncCommandImpl(async () => {
    var     view    = new LauncherIPDialog();
    object? @object = await DialogHost.Show(view, "LauncherIPDialogHost").ConfigureAwait(false);

    if (@object is not DialogResult<IPAddress?> result) {
      throw new Exception("Returned object from DialogHost is not typeof DialogResult!");
    }

    if (result.Result == MessageBoxResult.Cancel || result.Value is null) {
      Logger.Debug("Result: {0} | Value: {1}", result.Result, result.Value);

      return;
    }

    this.SetIP(result.Value);
    App.AccountManager.UpdateCurrentAccount(result.Value);
  });

  /// <summary>
  /// Gets the value of the add account command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand AddAccountCommand => new AsyncCommandImpl(async () => {
    var     view    = new AddAccountDialog();
    object? @object = await DialogHost.Show(view, "AddAccountDialogHost").ConfigureAwait(false);

    if (@object is not DialogResult<string> result) {
      throw new Exception("Returned object from DialogHost is not typeof DialogResult!");
    }

    if (result.Result == MessageBoxResult.Cancel || result.Value is null) {
      Logger.Debug("Result: {0} | Value: {1}", result.Result, result.Value);

      return;
    }

    var account = App.AccountManager.AddAccount(result.Value);
    App.AccountManager.SwitchAccount(account.Id);
    this.Parent.ReloadSettings();
  });

  /// <summary>
  /// Gets the value of the rename account command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand RenameAccountCommand => new AsyncCommandImpl<Guid>(async (Guid id) => {
    var     view    = new RenameAccountDialog();
    object? @object = await DialogHost.Show(view, "RenameAccountDialogHost").ConfigureAwait(false);

    if (@object is not DialogResult<string> result) {
      throw new Exception("Returned object from DialogHost is not typeof DialogResult!");
    }

    if (result.Result == MessageBoxResult.Cancel || result.Value is null) {
      Logger.Debug("Result: {0} | Value: {1}", result.Result, result.Value);

      return;
    }

    App.AccountManager.RenameAccount(id, result.Value);
    this.RefreshData(updatePopupContent: true);
    this.Parent.ReloadSettings();
  });

  /// <summary>
  /// Gets the value of the switch account command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand SwitchAccountCommand => new CommandImpl<Guid>((Guid accountId) => App.AccountManager.SwitchAccount(accountId));

  /// <summary>
  /// Gets the value of the delete account command
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public ICommand DeleteAccountCommand => new AsyncCommandImpl<Guid>(async (Guid id) => {
    var     view    = new DeleteAccountDialog();
    object? @object = await DialogHost.Show(view, "DeleteAccountDialogHost").ConfigureAwait(false);

    if (@object is not DialogResult<bool> result) {
      throw new Exception("Returned object from DialogHost is not typeof DialogResult!");
    }

    if (result.Result == MessageBoxResult.Cancel || !result.Value) {
      Logger.Debug("Result: {0} | Value: {1}", result.Result, result.Value);

      return;
    }

    App.AccountManager.RemoveAccount(id);
    this.RefreshData(updatePopupContent: true);
  });
#endregion Commands

#region Switch Account Row Calculations
  /// <summary>
  /// The grid actual height
  /// </summary>
  private double _gridActualHeight;

  /// <summary>
  /// Gets or sets the value of the grid actual height
  /// </summary>
  public double GridActualHeight {
    get => this._gridActualHeight;
    set => this.SetProperty(ref this._gridActualHeight, value);
  }

  /// <summary>
  /// The button margin
  /// </summary>
  private Thickness _buttonMargin;

  /// <summary>
  /// Gets or sets the value of the button margin
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public Thickness ButtonMargin {
    get => this._buttonMargin;
    set => this.SetProperty(ref this._buttonMargin, value);
  }

  /// <summary>
  /// The button height width
  /// </summary>
  private double _buttonHeightWidth;

  /// <summary>
  /// Gets or sets the value of the button height width
  /// </summary>
  public double ButtonHeightWidth {
    get => this._buttonHeightWidth;
    set => this.SetProperty(ref this._buttonHeightWidth, value);
  }

  /// <summary>
  /// The button icon margin
  /// </summary>
  private Thickness _buttonIconMargin;

  /// <summary>
  /// Gets or sets the value of the button icon margin
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  public Thickness ButtonIconMargin {
    get => this._buttonIconMargin;
    set => this.SetProperty(ref this._buttonIconMargin, value);
  }

  /// <summary>
  /// The button icon height width
  /// </summary>
  private double _buttonIconHeightWidth;

  /// <summary>
  /// Gets or sets the value of the button icon height width
  /// </summary>
  public double ButtonIconHeightWidth {
    get => this._buttonIconHeightWidth;
    set => this.SetProperty(ref this._buttonIconHeightWidth, value);
  }

  /// <summary>
  /// The account selection button actual width
  /// </summary>
  private double _accountSelectionButtonActualWidth;

  /// <summary>
  /// Gets or sets the value of the account selection button actual width
  /// </summary>
  public double AccountSelectionButtonActualWidth {
    get => this._accountSelectionButtonActualWidth;
    set => this.SetProperty(ref this._accountSelectionButtonActualWidth, value);
  }
#endregion Switch Account Row Caluclations

#region Disposable Collections
  /// <summary>
  /// Gets the value of the dispose of size changed
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global"), SuppressMessage("Performance", "CA1822:Mark members as static")]
  private Dictionary<string, FrameworkElement> DisposeOfSizeChanged => [];
#endregion Disposable Collections
#endregion Binding Properties

#region Constructors
  /// <summary>
  /// Initializes a new instance of the <see cref="SettingsControlViewModel"/> class
  /// </summary>
  /// <param name="settingsControl">The settings control</param>
  internal SettingsControlViewModel(SettingsControl settingsControl) : base(settingsControl) {
    this.SetOTPKey(App.AccountManager.CurrentAccount?.Token is not null);

    if (App.AccountManager.CurrentAccount is not null) {
      this.SetIP(App.AccountManager.CurrentAccount.LauncherIpAddress);
    }

    this.BuildCredits();
    this.UpdatePopupContent();
    this.NotifyPropertyChanged(nameof(this.CurrentAccountName));
    App.AccountManager.AccountSwitched += this.AccountManager_OnAccountSwitched;
    App.AccountManager.ReloadTriggered += this.AccountManager_OnReloadTriggered;
  }

#if DEBUG
  [SuppressMessage("ReSharper", "UnusedMember.Global"), SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."), SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  public SettingsControlViewModel() {
    this.SetIP(arg: null);
    this.SetOTPKey(value: false);
  }
#endif
  #endregion Constructors

#region Internal Methods
#region Override Methods
  /// <summary>
  /// Sets the property using the specified member
  /// </summary>
  /// <typeparam name="T">The </typeparam>
  /// <param name="member">The member</param>
  /// <param name="value">The value</param>
  /// <param name="propertyName">The property name</param>
  /// <returns>The bool</returns>
  [SuppressMessage("Design", "MA0051:Method is too long", Justification = "80 out of 60 maximum lines, good enough.")]
  protected override bool SetProperty<T>(ref T member, T value, [CallerMemberName] string propertyName = "") {
    switch (propertyName) {
      case "OTPKey":
        this.SetOTPKey(value is string);
        goto default;
      case "LauncherIPValue":
        if (value is IPAddress launcherIP1) {
          this.SetIP(launcherIP1);
        } else if (value is string launcherIPString && IPAddress.TryParse(launcherIPString, out IPAddress? launcherIP2)) {
          this.SetIP(launcherIP2);
        } else {
          Logger.ErrorDialog(exception: null, "Unable to parse IP address '{0}'{1}", value, value is not string and not null ? string.Format(" [{0}]", value.GetType().FullName) : string.Empty);
          return false;
        }
        goto default;
      case nameof(this.AccountSelectionButtonActualWidth): {
        if (value is double @double) {
          Logger.Verbose("AccountSelectionButtonActualWidth = {0}d", @double);
        }
        goto default;
      }
      case nameof(this.GridActualHeight): {
        if (value is double @double) {
          double unit = @double / (4.0d / 3.0d);
          Logger.Verbose("GridActualHeight = {0}d", @double);
          this.ButtonHeightWidth = unit;
        } else {
          return false;
        }
        goto default;
      }
      case nameof(this.ButtonHeightWidth): {
        if (value is double @double) {
          Logger.Verbose("ButtonHeightWidth = {0}d", @double);
          double unit1 = (this.GridActualHeight - @double) / 2.0d;
          this.ButtonMargin = new Thickness(unit1, unit1, unit1, unit1);
          this.ButtonIconHeightWidth = @double / 1.5d;
        } else {
          return false;
        }
        goto default;
      }
      case nameof(this.ButtonIconHeightWidth): {
        if (value is double @double) {
          Logger.Verbose("ButtonIconHeightWidth = {0}d", @double);
          if (this.ButtonHeightWidth != 0.0d) {
            double unit = (this.ButtonHeightWidth - @double) / 2.0d;
            this.ButtonIconMargin = new Thickness(unit, unit, unit, unit);
          } else {
            return false;
          }
        }
        goto default;
      }
      case nameof(this.ButtonMargin): {
        if (value is Thickness thickness) {
          Logger.Verbose("ButtonMargin = {0}d", thickness.Bottom);
        }
        goto default;
      }
      case nameof(this.ButtonIconMargin): {
        if (value is Thickness thickness) {
          Logger.Verbose("ButtonIconMargin = {0}d", thickness.Bottom);
        }
        goto default;
      }
      default:
        return base.SetProperty(ref member, value, propertyName);
    }
  }
#endregion Override Methods

#region Dispose Methods
  /// <inheritdoc />
  protected override void ReleaseUnmanagedResources() {
    foreach ((string method, FrameworkElement frameworkElement) in this.DisposeOfSizeChanged) {
      if (method.Equals(nameof(AccountSelectionButtonActualWidth_SizeChanged), StringComparison.OrdinalIgnoreCase)) {
        frameworkElement.SizeChanged -= this.AccountSelectionButtonActualWidth_SizeChanged;
      } else {
        if (method.Equals(nameof(GridActualHeight_SizeChanged), StringComparison.OrdinalIgnoreCase)) {
          frameworkElement.SizeChanged -= this.GridActualHeight_SizeChanged;
        } else {
          Logger.Warning("Attempted to dispose item of {0}.{1} method name of {2}", nameof(SettingsControlViewModel), nameof(this.DisposeOfSizeChanged), method);
        }
      }
    }

    base.ReleaseUnmanagedResources();
  }
  #endregion Dispose Methods

  /// <summary>
  /// Refreshes the data on this view model.
  /// </summary>
  /// <param name="updatePopupContent">A boolean determine whether to update the update pop-up content.</param>
  /// <param name="updateOTPKeyContent">A boolean determine whether to update the update OTP key content.</param>
  /// <param name="updateLauncherIPContent">A boolean determine whether to update the update launcher IP content.</param>
  /// <param name="updateLabels">A boolean determine whether to update all the labels.</param>
  [SuppressMessage("Maintainability", "AV1564:Parameter in public or internal member is of type bool or bool?")]
  internal void RefreshData(bool updatePopupContent = false, bool updateOTPKeyContent = false, bool updateLauncherIPContent = false, bool updateLabels = false) {
    base.RefreshData();

    if (updatePopupContent) {
      this.UpdatePopupContent();
    }

    if (updateLabels) {
      this.UpdateLabels();
    }

    // ReSharper disable once InvertIf
    if (App.AccountManager.CurrentAccount is not null) {
      if (updateOTPKeyContent) {
        this.SetOTPKey(App.AccountManager.CurrentAccount.Token is not null);
      }

      if (updateLauncherIPContent) {
        this.SetIP(App.AccountManager.CurrentAccount.LauncherIpAddress);
      }
    }
  }
#endregion Internal Methods

#region Private Methods
#region Event Handlers
  /// <summary>
  /// Accounts the manager on account switched using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void AccountManager_OnReloadTriggered(object? sender, EventArgs e) {
    this.RefreshData(updatePopupContent: true, updateOTPKeyContent: true, updateLauncherIPContent: true, updateLabels: true);
  }

  /// <summary>
  /// Accounts the manager on account switched using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void AccountManager_OnAccountSwitched(object? sender, AccountSwitchedEventArgs e) {
    this.NotifyPropertyChanged(nameof(this.CurrentAccountName));
    (this.Parent.ParentWindow as MainWindow)?.SettingsControl.RefreshData();

    // ReSharper disable once InvertIf
    if (e.NewAccount is not null) {
      this.SetOTPKey(e.NewAccount.Token is not null);
      this.SetIP(e.NewAccount.LauncherIpAddress);
    }
  }

  /// <summary>
  /// Accounts the selection button actual width size changed using the specified
  /// </summary>
  /// <param name="_">The </param>
  /// <param name="args">The args</param>
  private void AccountSelectionButtonActualWidth_SizeChanged(object? _, SizeChangedEventArgs args) {
    this.AccountSelectionButtonActualWidth = args.NewSize.Width - 48;
  }

  /// <summary>
  /// Grids the actual height size changed using the specified
  /// </summary>
  /// <param name="_">The </param>
  /// <param name="args">The args</param>
  private void GridActualHeight_SizeChanged(object? _, SizeChangedEventArgs args) {
    this.GridActualHeight = args.NewSize.Height;
  }
#endregion Event Handlers
  /// <summary>
  /// Updates the dialog's labels
  /// </summary>
  private void UpdateLabels() {
    foreach (PropertyInfo property in this.GetType().GetProperties()) {
      if (property.PropertyType == typeof(string)) {
        this.NotifyPropertyChanged(property.Name);
      }
    }
  }

  /// <summary>
  /// Updates the pop-up content
  /// </summary>
  // TODO: See if this can be condensed.
  [SuppressMessage("Design", "MA0051:Method is too long", Justification = "This is heavy I know.")]
  private void UpdatePopupContent() {
#region StackPanel Outer
    this.Parent.AccountSelectionButton.SizeChanged += this.AccountSelectionButtonActualWidth_SizeChanged;
    this.DisposeOfSizeChanged.Add(nameof(SettingsControlViewModel.AccountSelectionButtonActualWidth_SizeChanged), this.Parent.AccountSelectionButton);

    // ReSharper disable once PatternAlwaysMatches
    var stackPanelOuter = new StackPanel {
      Orientation = Orientation.Vertical,
      HorizontalAlignment = HorizontalAlignment.Stretch,
      CanHorizontallyScroll = false,
      CanVerticallyScroll = false,
      Resources = new ResourceDictionary {
        {
          typeof(ScrollViewer), new Style(typeof(ScrollViewer), App.GetStyle("MaterialDesignScrollViewer"))
        }, {
          typeof(ScrollBar), new Style(typeof(ScrollBar), App.GetStyle("MaterialDesignScrollBarMinimal"))
        },
      },
    };

    BindingOperations.SetBinding(stackPanelOuter, FrameworkElement.MaxWidthProperty, new Binding {
      Source = this,
      Path = new PropertyPath(nameof(this.AccountSelectionButtonActualWidth)),
    });

    BindingOperations.SetBinding(stackPanelOuter, FrameworkElement.WidthProperty, new Binding {
      Source = this,
      Path = new PropertyPath(nameof(this.AccountSelectionButtonActualWidth)),
    });

#region Add Account Button
    var stackPanelButton = new StackPanel {
      Orientation = Orientation.Horizontal,
      HorizontalAlignment = HorizontalAlignment.Center,
    };

    stackPanelButton.Children.Add(new PackIcon {
      Kind = PackIconKind.AccountAdd,
    });

    stackPanelButton.Children.Add(new TextBlock {
      Text = Loc.Localize("AddAccountButtonLabel", "Add Account"),
      Margin = new Thickness(10, 0, 0, 0),
    });

    var addAccountButton = new Button {
      Content = stackPanelButton,
      Foreground = App.GetBrush("MaterialDesign.Brush.Foreground"),
      HorizontalAlignment = HorizontalAlignment.Stretch,
    };

    BindingOperations.SetBinding(addAccountButton, ButtonBase.CommandProperty, new Binding {
      Source = this,
      Path = new PropertyPath(nameof(this.AddAccountCommand)),
    });
#endregion Add Account Button

#region Scroll Viewer
    var scrollViewer = new ScrollViewer {
      HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden,
      VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
      MaxHeight = 250.0d,
    };

#region StackPanel Inner
    var stackPanelInner = new StackPanel {
      Orientation = Orientation.Vertical,
      HorizontalAlignment = HorizontalAlignment.Stretch,
      CanHorizontallyScroll = false,
      CanVerticallyScroll = true,
    };

#region StackPanel Inner Row
    foreach (TOTPAccount account in App.AccountManager.Accounts) {
      var row = new Grid {
        ColumnDefinitions = {
          new ColumnDefinition {
            Width = new GridLength(1.00, GridUnitType.Star),
          },
          new ColumnDefinition {
            Width = GridLength.Auto,
          },
          new ColumnDefinition {
            Width = GridLength.Auto,
          },
        },
      };

      row.SizeChanged += this.GridActualHeight_SizeChanged;
      this.DisposeOfSizeChanged.Add(nameof(SettingsControlViewModel.GridActualHeight_SizeChanged), row);

#region Account Button
      var accountButton = new Button {
        HorizontalContentAlignment = HorizontalAlignment.Left,
        VerticalContentAlignment = VerticalAlignment.Bottom,
        CommandParameter = account.Id,
        Content = account.Name,
        Foreground = App.GetBrush("MaterialDesign.Brush.Foreground"),
      };

      BindingOperations.SetBinding(accountButton, ButtonBase.CommandProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.SwitchAccountCommand)),
      });

      if (account.IsCurrent) {
        accountButton.IsEnabled = false;
      }

      row.Children.Add(accountButton);
      Grid.SetColumn(accountButton, 0);
      Grid.SetColumnSpan(accountButton, 3);
#endregion

#region Rename Button
      var renameAccountButton = new Button {
        Height = double.NaN, // Auto
        HorizontalAlignment = HorizontalAlignment.Right,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Top,
        CommandParameter = account.Id,
        Foreground = App.GetBrush("MaterialDesign.Brush.Foreground"),
      };

      BindingOperations.SetBinding(renameAccountButton, ButtonBase.CommandProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.RenameAccountCommand)),
      });

      BindingOperations.SetBinding(renameAccountButton, FrameworkElement.WidthProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(renameAccountButton, FrameworkElement.HeightProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(renameAccountButton, FrameworkElement.MarginProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonMargin)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(renameAccountButton, Control.PaddingProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconMargin)),
        Mode = BindingMode.TwoWay,
      });

#region Rename Button Icon
      var renameAccountButtonIcon = new PackIcon {
        Kind = PackIconKind.Edit,
      };

      BindingOperations.SetBinding(renameAccountButtonIcon, FrameworkElement.WidthProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(renameAccountButtonIcon, FrameworkElement.HeightProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      renameAccountButton.Content = renameAccountButtonIcon;
#endregion Rename Button Icon

      row.Children.Add(renameAccountButton);
      Grid.SetColumn(renameAccountButton, 1);
#endregion Rename Button

#region Delete Button
      var deleteAccountButton = new Button {
        Height = double.NaN, // Auto
        Padding = new Thickness(0, 0, 0, 0),
        HorizontalAlignment = HorizontalAlignment.Right,
        VerticalAlignment = VerticalAlignment.Center,
        HorizontalContentAlignment = HorizontalAlignment.Center,
        VerticalContentAlignment = VerticalAlignment.Top,
        CommandParameter = account.Id,
        Foreground = App.GetBrush("MaterialDesign.Brush.Foreground"),
      };

      BindingOperations.SetBinding(deleteAccountButton, ButtonBase.CommandProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.DeleteAccountCommand)),
      });

      BindingOperations.SetBinding(deleteAccountButton, FrameworkElement.WidthProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(deleteAccountButton, FrameworkElement.HeightProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(deleteAccountButton, FrameworkElement.MarginProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonMargin)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(deleteAccountButton, Control.PaddingProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconMargin)),
        Mode = BindingMode.TwoWay,
      });

#region Delete Button Icon
      var deleteAccountButtonIcon = new PackIcon {
        Kind = PackIconKind.TrashCan,
      };

      BindingOperations.SetBinding(deleteAccountButtonIcon, FrameworkElement.WidthProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      BindingOperations.SetBinding(deleteAccountButtonIcon, FrameworkElement.HeightProperty, new Binding {
        Source = this,
        Path = new PropertyPath(nameof(this.ButtonIconHeightWidth)),
        Mode = BindingMode.TwoWay,
      });

      deleteAccountButton.Content = deleteAccountButtonIcon;
#endregion Delete Button Icon

      row.Children.Add(deleteAccountButton);
      Grid.SetColumn(deleteAccountButton, 2);
#endregion Delete Button

      stackPanelInner.Children.Add(row);
    }
#endregion StackPanel Inner Row
#endregion StackPanel Inner

    scrollViewer.Content = stackPanelInner;
#endregion Scroll Viewer

#region Divider
    var divider = new Rectangle {
      Fill = App.GetBrush("MaterialDesign.Brush.Foreground"),
      Height = 1,
    };

    BindingOperations.SetBinding(divider, FrameworkElement.WidthProperty, new Binding {
      Source = this,
      Path = new PropertyPath(nameof(this.AccountSelectionButtonActualWidth)),
      Mode = BindingMode.TwoWay,
    });
#endregion Divider

    stackPanelOuter.Children.Add(scrollViewer);
    stackPanelOuter.Children.Add(divider);
    stackPanelOuter.Children.Add(addAccountButton);
#endregion StackPanel Outer

    this.AccountSelectionButtonContent = stackPanelOuter;
  }

#region Private Static Methods
  /// <summary>
  /// Checks the IP using the specified args
  /// </summary>
  /// <param name="args">The args</param>
  /// <param name="onRun">The on run</param>
  /// <param name="onFinish">The on finish</param>
  private static void CheckIP(IPAddress args, Action onRun, Action<bool, Exception?> onFinish) {
    onRun();
    var finished = false;

    try {
      Logger.Debug("Checking IP: {0}", args);
      // ReSharper disable once SuggestVarOrType_SimpleTypes
      ConfiguredTaskAwaitable<PingReply>.ConfiguredTaskAwaiter pongTask = new Ping().SendPingAsync(args).ConfigureAwait(true).GetAwaiter();

      pongTask.UnsafeOnCompleted(() => {
        try {
          PingReply pong = pongTask.GetResult();
          finished = pong.Status.Equals(IPStatus.Success);
          Logger.Debug("Ping Completed: {0}", pong.Status);
          onFinish(finished, arg2: null);
        } catch (Exception e) {
          Logger.Error(e, "Error: Failed to ping {0}", args);
          onFinish(finished, e);
        }
      });
    } catch (Exception e) {
      Logger.Error(e, "Error: Failed to ping {0}", args);
      onFinish(finished, e);
    }
  }
#endregion Private Static Methods

  /// <summary>
  /// Builds the credits
  /// </summary>
  private void BuildCredits() {
    this.Parent.CreditsBlock.Inlines.Add("By NekoBoiNick, see ");

    var licenses = new Hyperlink {
      Foreground = App.GetBrush("MaterialDesign.Brush.Primary.Light"),
      Style = App.GetStyle("MaterialDesignCaptionHyperlink"),
      NavigateUri = new Uri("https://github.com/thakyz/XLAuthenticatorNet/master/LICENSE"),
    };

    licenses.Inlines.Add("licenses");
    this.Parent.CreditsBlock.Inlines.Add(licenses);
    this.Parent.CreditsBlock.Inlines.Add(" and ");

    var github = new Hyperlink {
      Foreground = App.GetBrush("MaterialDesign.Brush.Primary.Light"),
      Style = App.GetStyle("MaterialDesignCaptionHyperlink"),
      NavigateUri = new Uri("https://github.com/thakyz/XLAuthenticatorNet"),
    };

    github.Inlines.Add("source code");
    this.Parent.CreditsBlock.Inlines.Add(github);
    this.Parent.CreditsBlock.Inlines.Add(".");
  }

  /// <summary>
  /// Sets the OTP key using the specified value
  /// </summary>
  /// <param name="value">The value</param>
  private void SetOTPKey(bool value) {
    if (value) {
      this.IsRegisteredText = Loc.Localize("OTPKeyIsRegisteredNo", "Yes");
      this.IsRegisteredColor = new SolidColorBrush(Colors.LimeGreen);
    } else {
      this.IsRegisteredText = Loc.Localize("OTPKeyIsRegisteredNo", "No");
      this.IsRegisteredColor = new SolidColorBrush(Colors.Red);
    }
  }

  /// <summary>
  /// Sets the IP using the specified arg
  /// </summary>
  /// <param name="arg">The arg</param>
  private void SetIP(IPAddress? arg) {
    if (arg is null) {
      this.LauncherIPText = "N/A";
      this.LauncherIPColor = new SolidColorBrush(Colors.Red);

      return;
    }

    SettingsControlViewModel.CheckIP(arg, OnRun, OnFinish);

    return;

    void OnRun() {
      this.LauncherIPText = "...";
      this.LauncherIPColor = new SolidColorBrush(Colors.Gray);
    }

    void OnFinish(bool output, Exception? exception) {
      if (exception is not null) {
        Logger.Error(exception, "Task to check IP {0} canceled!", arg);
        this.LauncherIPText = arg.ToString();
        this.LauncherIPColor = new SolidColorBrush(Colors.Goldenrod);
      } else {
        if (!output) {
          this.LauncherIPText = arg.ToString();
          this.LauncherIPColor = new SolidColorBrush(Colors.Red);
        } else {
          this.LauncherIPText = arg.ToString();
          this.LauncherIPColor = new SolidColorBrush(Colors.LimeGreen);
        }
      }
    }
  }
#endregion Private Methods
}
