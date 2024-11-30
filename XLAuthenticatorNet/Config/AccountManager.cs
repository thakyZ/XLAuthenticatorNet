using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Serilog;
using XLAuthenticatorNet.Domain;
using XLAuthenticatorNet.Extensions;
using XLAuthenticatorNet.Models.Events;
using XLAuthenticatorNet.Windows;

namespace XLAuthenticatorNet.Config;

/// <summary>
/// The account manager class
/// </summary>
internal class AccountManager {
  internal event EventHandler<AccountSwitchedEventArgs>? AccountSwitched;
  internal event EventHandler?                           ReloadTriggered;

  /// <summary>
  /// Gets the value of the accounts
  /// </summary>
  internal ObservableCollection<TotpAccount> Accounts { get; }

  /// <summary>
  /// The current account
  /// </summary>
  private TotpAccount? _currentAccount;
  /// <summary>
  /// Gets or sets the value of the current account
  /// </summary>
  public TotpAccount? CurrentAccount {
    get => this.Accounts.FirstOrDefault(a => a.Id == App.Settings.CurrentAccountID);
    private set {
      if (value is not null && App.Settings.CurrentAccountID == value.Id) {
        return;
      }
      this._currentAccount = value;
      App.Settings.CurrentAccountID = value?.Id ?? Guid.Empty;
      AccountSwitched?.Invoke(null, new AccountSwitchedEventArgs(this._currentAccount, value));
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AccountManager"/> class
  /// </summary>
  internal AccountManager() {
    this.Accounts = [..Load()];
    this.Accounts.CollectionChanged += Accounts_CollectionChanged;
  }

  internal void NotifyReloadTriggered([CallerMemberName] string caller = "") {
    this.ReloadTriggered?.Invoke(caller, EventArgs.Empty);
  }

  /// <summary>
  /// Accountses the collection changed using the specified sender
  /// </summary>
  /// <param name="sender">The sender</param>
  /// <param name="e">The </param>
  private void Accounts_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
    this.ReloadTriggered?.Invoke(nameof(AccountManager), EventArgs.Empty);
  }

  /// <summary>
  /// Updates the current account using the specified value
  /// </summary>
  /// <typeparam name="TProperty">The property</typeparam>
  /// <param name="value">The value</param>
  internal void UpdateCurrentAccount<TProperty>(TProperty? value) {
    if (value is null || this.CurrentAccount is null) {
      return;
    }

    Type type = typeof(TProperty);

    if (type.IsEquivalentTo(typeof(IPAddress))) {
      this.CurrentAccount.LauncherIpAddress = value as IPAddress;
    } else if (type.IsEquivalentTo(typeof(string))) {
      if (IPAddress.TryParse(value as string, out IPAddress? address)) {
        this.CurrentAccount.LauncherIpAddress = address;
      } else {
        RenameAccount(this.CurrentAccount.Id, value as string);
      }
    } else if (type.IsEquivalentTo(typeof(SecureString))) {
      Log.Debug("UpdatePassword() called");
      this.CurrentAccount.Token = value as SecureString;
    }
    AccountSwitched?.Invoke(null, new AccountSwitchedEventArgs(null, this.CurrentAccount));
  }

  /// <summary>
  /// Switches the account using the specified account
  /// </summary>
  /// <param name="account">The account</param>
  /// <param name="saveAsCurrent">The save as current</param>
  internal void SwitchAccount(TotpAccount? account, bool saveAsCurrent) {
    App.ReloadSettings();
    App.RefreshData(all: true);

    if (saveAsCurrent) {
      this.SetCurrentAccount(account);
    }
  }

  /// <summary>
  /// Switches the account using the specified account id
  /// </summary>
  /// <param name="accountId">The account id</param>
  internal void SwitchAccount(Guid accountId) {
    if (this.Accounts.FirstOrDefault(a => a.Id == accountId) is not TotpAccount account) {
      return;
    }
    App.ReloadSettings();
    App.RefreshData(all: true);
    this.SetCurrentAccount(account);
  }

  /// <summary>
  /// Adds the account using the specified name
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="account">The account</param>
  /// <param name="ipAddress">The ip address</param>
  /// <param name="totpKey">The totp key</param>
  internal void AddAccount(string name, out TotpAccount account, IPAddress? ipAddress = null, SecureString? totpKey = null) {
    account = new TotpAccount(name, ipAddress, totpKey);
    this.AddAccount(account);
  }

  /// <summary>
  /// Adds the account using the specified account
  /// </summary>
  /// <param name="account">The account</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal void AddAccount(TotpAccount account) {
    TotpAccount? existingAccount = this.Accounts.FirstOrDefault(a => a.Id == account.Id);
    Log.Debug($"Existing Account: {existingAccount?.Id}");

    if (existingAccount is not null && existingAccount.Token != account.Token) {
      Log.Debug("Updating password...");
      existingAccount.Token = account.Token;
      return;
    }

    if (existingAccount is not null) {
      return;
    }

    this.Accounts.Insert(0, account);
  }

  /// <summary>
  /// Renames the account using the specified account id
  /// </summary>
  /// <param name="accountID">The account id</param>
  /// <param name="value">The value</param>
  internal void RenameAccount(Guid accountID, string? value) {
    if (this.Accounts.FirstOrDefault(x => x.Id == accountID) is not TotpAccount account || value.IsNullOrEmptyOrWhiteSpace()) {
      return;
    }

    account.Name = value;
  }

  /// <summary>
  /// Removes the account using the specified account id
  /// </summary>
  /// <param name="accountID">The account id</param>
  internal void RemoveAccount(Guid accountID) {
    if (this.Accounts.Count == 1) {
      return;
    }

    if (this.CurrentAccount?.Id.Equals(accountID) == true) {
      if (this.Accounts.FirstOrDefault(x => !x.Id.Equals(accountID)) is TotpAccount totpAccount) {
        this.SwitchAccount(totpAccount.Id);
      } else {
        return;
      }
    }

    this.Accounts.RemoveWhere(x => x.Id == accountID);
  }

  /// <summary>
  /// Removes the account using the specified account
  /// </summary>
  /// <param name="account">The account</param>
  internal void RemoveAccount(TotpAccount account) {
    this.Accounts.Remove(account);
  }

  #region Save & Load

  /// <summary>
  /// The base directory
  /// </summary>
  private static readonly string _configurationPath = Path.Combine(Paths.RoamingPath, "accountList.json");

  /// <summary>
  /// Saves this instance
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal void Save() {
    string text = JsonConvert.SerializeObject(this.Accounts, App.SerializerSettings);
    File.WriteAllText(_configurationPath, text);
  }

  /// <summary>
  /// Saves the accounts
  /// </summary>
  /// <param name="accounts">The accounts</param>
  private static void Save(ObservableCollection<TotpAccount> accounts) {
    string text = JsonConvert.SerializeObject(accounts, App.SerializerSettings);
    File.WriteAllText(_configurationPath, text);
  }

  /// <summary>
  /// Loads
  /// </summary>
  /// <returns>A list of totp account</returns>
  private static List<TotpAccount> Load() {
    if (!File.Exists(_configurationPath)) {
      Save([]);
    }

    List<TotpAccount>? output = [];

    try {
      using FileStream fs   = File.OpenRead(_configurationPath);
      using var sr   = new StreamReader(fs);
      string       text = sr.ReadToEnd();

      output = JsonConvert.DeserializeObject<List<TotpAccount>>(text, App.SerializerSettings);
#if DEBUG
      if (output is null || output.Count == 0) {
        var testAccount = new TotpAccount("Test", new IPAddress([127, 0, 0, 1]), "a01b23c45d67e89f".ToSecureString());
        return [testAccount];
      }
      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#endif
    } catch (Exception exception) {
      _ = CustomMessageBox.AssertOrShowError(true, "Failed to load accountList.json\n" + exception.Message + (exception.StackTrace is string stackTrace ? "\n" + stackTrace : string.Empty), true);
    }

    return output ?? [];
  }

  #endregion Save & Load

  /// <summary>
  /// Sets the current account using the specified account
  /// </summary>
  /// <param name="account">The account</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal void SetCurrentAccount(TotpAccount? account) {
    this.CurrentAccount = this.Accounts.FirstOrDefault(a => a.Id == account?.Id);
  }

  /// <summary>
  /// Sends the otp key using the specified otp value
  /// </summary>
  /// <param name="otpValue">The otp value</param>
  /// <returns>A task containing the otp key response response exception exception</returns>
  internal static async Task<(OTPKeyResponse Response, Exception? Exception)> SendOTPKey(string? otpValue) {
    Exception? exception = null;
    if (App.AccountManager.CurrentAccount is null) {
      return (OTPKeyResponse.CurrentAccountNull, exception);
    }

    if (App.AccountManager.CurrentAccount.LauncherIpAddress is null) {
      return (OTPKeyResponse.LauncherIpAddressNull, exception);
    }

    if (otpValue.IsNullOrEmptyOrWhiteSpace() || !otpValue.IsNumberOf(digits: 6)) {
      return (OTPKeyResponse.OTPValueNull, exception);
    }

    using var httpClient = new HttpClient();
    IXLLauncherResponse? result = null;
    try {
      result = await httpClient.GetFromJsonAsync<IXLLauncherResponse>($"http://{App.AccountManager.CurrentAccount.LauncherIpAddress}:4646/ffxivlauncher/{otpValue}", App.SerializerSettings);
    } catch (Exception ex) {
      Log.Error(ex, "Failed to send otp key to XLLauncher");
      exception = ex;
    }

    if (result is IXLLauncherResponse response && response.App.Equals("XIVLauncher", StringComparison.OrdinalIgnoreCase)) {
      return (OTPKeyResponse.OTPValueNull, exception);
    }

    return (OTPKeyResponse.Failed, exception);
  }
}