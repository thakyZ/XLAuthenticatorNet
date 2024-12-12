using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Security;

using Newtonsoft.Json;

using XLAuth.Domain;
using XLAuth.Extensions;
using XLAuth.Models.Abstracts;
using XLAuth.Models.Events;
using XLAuth.Windows;

namespace XLAuth.Config;

/// <summary>
/// The item manager class
/// </summary>
internal sealed class AccountManager {
  internal event EventHandler<AccountSwitchedEventArgs>? AccountSwitched;
  internal event EventHandler?                           ReloadTriggered;

  /// <summary>
  /// Gets the value of the accounts
  /// </summary>
  internal ObservableCollection<TOTPAccount> Accounts { get; }

  /// <summary>
  /// The current item
  /// </summary>
  private TOTPAccount? _currentAccount;

  /// <summary>
  /// Gets or sets the value of the current item
  /// </summary>
  public TOTPAccount? CurrentAccount {
    get => this.Accounts.FirstOrDefault(account => account.Id == App.Settings.CurrentAccountID);
    private set {
      if (value is not null && App.Settings.CurrentAccountID == value.Id) {
        return;
      }
      var previousAccount = this._currentAccount;
      this._currentAccount = value;
      App.Settings.CurrentAccountID = value?.Id ?? Guid.Empty;
      this.OnAccountSwitched(previousAccount, value);
    }
  }

  /// <summary>
  /// Initializes item new instance of the <see cref="AccountManager"/> class
  /// </summary>
  internal AccountManager() {
    this.Accounts = [..AccountManager.Load()];
    this.Accounts.CollectionChanged += this.OnCollectionChanged;
  }

  /// <summary>
  /// Trigged when the reload of the <see cref="AccountManager" /> is directed to be triggered.
  /// </summary>
  /// <param name="caller">The name of the member that called this class.</param>
  internal void OnReloadTriggered([CallerMemberName] string caller = "") {
    var eventArgs = new PropertyChangedEventArgs(caller);
    this.ReloadTriggered?.Invoke(this, eventArgs);
  }

  /// <summary>
  /// Handles if the collection of <see cref="Accounts"/> is changed.
  /// </summary>
  /// <param name="sender">The sender of this event.</param>
  /// <param name="event">The <see cref="NotifyCollectionChangedEventArgs" /> event args.</param>
  private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs @event) {
    this.OnReloadTriggered();
  }

  /// <summary>
  /// Updates the current item using the specified value
  /// </summary>
  /// <typeparam name="TProperty">The property</typeparam>
  /// <param name="value">The value</param>
  internal void UpdateCurrentAccount<TProperty>(TProperty? value) {
    if (value is null || this.CurrentAccount is null) {
      return;
    }

    var type = typeof(TProperty);

    if (type.IsEquivalentTo(typeof(IPAddress))) {
      this.CurrentAccount.LauncherIpAddress = value as IPAddress;
    } else {
      if (type.IsEquivalentTo(typeof(string))) {
        if (IPAddress.TryParse(value as string, out var address)) {
          this.CurrentAccount.LauncherIpAddress = address;
        } else {
          this.RenameAccount(this.CurrentAccount.Id, value as string);
        }
      } else {
        if (type.IsEquivalentTo(typeof(SecureString))) {
          Logger.Debug("UpdatePassword() called");
          this.CurrentAccount.Token = value as SecureString;
        }
      }
    }

    this.OnAccountSwitched(previous: null, this.CurrentAccount);
  }

  private void OnAccountSwitched(TOTPAccount? previous, TOTPAccount? current) {
    var eventArgs = new AccountSwitchedEventArgs(previous, current);
    this.AccountSwitched?.Invoke(this, eventArgs);
  }

  /// <summary>
  /// Switches the item using the specified item
  /// </summary>
  /// <param name="account">The item</param>
  /// <param name="saveAsCurrent">The save as current</param>
  internal void SwitchAccount(TOTPAccount? account, bool saveAsCurrent) {
    App.ReloadSettings();
    App.RefreshData(RefreshPart.UpdateAll);

    if (!saveAsCurrent) {
      return;
    }

    this.SetCurrentAccount(account);
  }

  /// <summary>
  /// Switches the item using the specified item id
  /// </summary>
  /// <param name="accountId">The item id</param>
  internal void SwitchAccount(Guid accountId) {
    if (this.Accounts.FirstOrDefault(account => account.Id == accountId) is not TOTPAccount account) {
      return;
    }

    this.SwitchAccount(account, saveAsCurrent: false);
  }

  /// <summary>
  /// Adds the item using the specified item
  /// </summary>
  /// <param name="account">The item</param>
  internal void AddOrUpdateAccount(TOTPAccount account) {
    var existingAccount = this.Accounts.FirstOrDefault(account => account.Id == account.Id);
    Logger.Debug("Existing Account: {0}", existingAccount?.Id);

    if (existingAccount is not null && existingAccount.Token != account.Token) {
      Logger.Debug("Updating password...");
      existingAccount.Token = account.Token;
      return;
    }

    if (existingAccount is not null) {
      return;
    }

    this.Accounts.Insert(0, account);
  }

  /// <summary>
  /// Adds the item using the specified name
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="ipAddress">The IP address</param>
  /// <param name="totpKey">The TOTP key</param>
  internal TOTPAccount AddAccount(string name, IPAddress? ipAddress = null, SecureString? totpKey = null) {
    var account = new TOTPAccount(name, ipAddress, totpKey);
    this.AddOrUpdateAccount(account);
    return account;
  }

  /// <summary>
  /// Renames the item using the specified item id
  /// </summary>
  /// <param name="accountID">The item id</param>
  /// <param name="value">The value</param>
  internal void RenameAccount(Guid accountID, string? value) {
    if (this.Accounts.FirstOrDefault(account => account.Id == accountID) is not TOTPAccount account || value.IsNullOrEmptyOrWhiteSpace()) {
      return;
    }

    account.Name = value;
  }

  /// <summary>
  /// Removes the item using the specified item id
  /// </summary>
  /// <param name="accountID">The item id</param>
  internal void RemoveAccount(Guid accountID) {
    if (this.Accounts.Count == 1) {
      return;
    }

    if (this.CurrentAccount?.Id.Equals(accountID) == true) {
      if (this.Accounts.FirstOrDefault(account => !account.Id.Equals(accountID)) is TOTPAccount totpAccount) {
        this.SwitchAccount(totpAccount.Id);
      } else {
        return;
      }
    }

    this.Accounts.RemoveWhere(account => account.Id == accountID);
  }

  /// <summary>
  /// Removes the item using the specified item
  /// </summary>
  /// <param name="account">The item</param>
  internal void RemoveAccount(TOTPAccount account) {
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
  internal void Save() {
    AccountManager.Save(this.Accounts);
  }

  /// <summary>
  /// Saves the accounts
  /// </summary>
  /// <param name="accounts">The accounts</param>
  private static void Save(ObservableCollection<TOTPAccount> accounts) {
    var text = JsonConvert.SerializeObject(accounts, Util.SerializerSettings);
    File.WriteAllText(_configurationPath, text);
  }

  /// <summary>
  /// Loads
  /// </summary>
  /// <returns>A list of TOTP item</returns>
  private static List<TOTPAccount> Load() {
    if (!File.Exists(_configurationPath)) {
      AccountManager.Save([]);
    }

    List<TOTPAccount>? output = [];

    try {
      using var fs   = File.OpenRead(_configurationPath);
      using var sr   = new StreamReader(fs);
      var       text = sr.ReadToEnd();

      output = JsonConvert.DeserializeObject<List<TOTPAccount>>(text, Util.SerializerSettings);
#if DEBUG
      if (output is null || output.Count == 0) {
        var ipAddress = new IPAddress([127, 0, 0, 1]);
        var temporaryKey = "a01b23c45d67e89f".ToSecureString();
        var testAccount = new TOTPAccount("Test", ipAddress, temporaryKey);
        return [testAccount];
      }
      // ReSharper disable once ConditionIsAlwaysTrueOrFalse
#endif
    } catch (Exception exception) {
      _ = CustomMessageBox.AssertOrShowError(condition: true, "Failed to load accountList.json\n" + exception.ToFullyQualifiedString(), fatal: true);
    }

    return output ?? [];
  }
  #endregion Save & Load

  /// <summary>
  /// Sets the current account using the specified account
  /// </summary>
  /// <param name="account">The account</param>
  internal void SetCurrentAccount(TOTPAccount? account) {
    this.CurrentAccount = this.Accounts.FirstOrDefault(item => item.Id == account?.Id);
  }

  /// <summary>
  /// Sends the OTP key using the specified OTP value
  /// </summary>
  /// <param name="otpValue">The OTP value</param>
  /// <returns>A task containing the OTP key response exception</returns>
  internal static async Task<(OTPKeyResponse Response, Exception? Exception)> SendOTPKeyAsync(string? otpValue) {
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

    IXLLauncherResponse? result = null;
    try {
      result = await App.HttpClient.GetFromJsonAsync<IXLLauncherResponse>(string.Format(CultureInfo.InvariantCulture, "http://{0}:4646/ffxivlauncher/{1}", App.AccountManager.CurrentAccount.LauncherIpAddress, otpValue), Util.SerializerSettings).ConfigureAwait(false);
    } catch (Exception innerException) {
      Logger.Error(innerException, "Failed to send OTP key to XLLauncher");
      exception = innerException;
    }

    if (result is IXLLauncherResponse response && response.App.Equals("XIVLauncher", StringComparison.OrdinalIgnoreCase)) {
      return (OTPKeyResponse.Success, exception);
    }

    return (OTPKeyResponse.Failed, exception);
  }
}
