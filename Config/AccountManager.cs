using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

using Newtonsoft.Json;

namespace XLAuthenticatorNet.Config {

  public class AccountManager {
    public ObservableCollection<TotpAccount> Accounts;

    public TotpAccount CurrentAccount {
      get {
        return Accounts.Count > 1 ?
            Accounts.FirstOrDefault(a => a.Id == _setting.CurrentAccountId) :
            Accounts.FirstOrDefault();
      }

      set => _setting.CurrentAccountId = value.Id;
    }

    private readonly IAuthSettingsV1 _setting;

    public AccountManager(IAuthSettingsV1 setting) {
      Load();

      _setting = setting;

      Accounts.CollectionChanged += Accounts_CollectionChanged;
    }

    private void Accounts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
      Save();
    }

    public void UpdateToken(TotpAccount account, string token) {
      Console.WriteLine("UpdatePassword() called");
      var existingAccount = Accounts.FirstOrDefault(a => a.Id == account.Id);
      existingAccount.Token = token;
    }

    public void AddAccount(TotpAccount account) {
      var existingAccount = Accounts.FirstOrDefault(a => a.Id == account.Id);
      Console.WriteLine($"Existing Account: {existingAccount?.Id}");

      if (existingAccount != null && existingAccount.Token != account.Token) {
        Console.WriteLine("Updating password...");
        existingAccount.Token = account.Token;
        return;
      }

      if (existingAccount != null) {
        return;
      }

      Accounts.Insert(0, account);
    }

    public void RemoveAccount(TotpAccount account) {
      Accounts.Remove(account);
    }

    #region Save & Load

    private static readonly string ConfigurationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "accountList.json");

    public void Save() {
      File.WriteAllText(ConfigurationPath, JsonConvert.SerializeObject(Accounts, Formatting.Indented, new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Objects,
        TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
      }));
    }

    public void Load() {
      if (!File.Exists(ConfigurationPath)) {
        Accounts = new ObservableCollection<TotpAccount>();
        Save();
      }

      Accounts = JsonConvert.DeserializeObject<ObservableCollection<TotpAccount>>(File.ReadAllText(ConfigurationPath), new JsonSerializerSettings {
        TypeNameHandling = TypeNameHandling.Objects
      });

      if (Accounts is null) {
        new ObservableCollection<TotpAccount>();
      }
    }

    #endregion Save & Load
  }
}