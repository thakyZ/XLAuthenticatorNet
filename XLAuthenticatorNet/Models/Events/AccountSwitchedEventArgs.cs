using System;
using System.Diagnostics.CodeAnalysis;
using XLAuthenticatorNet.Config;

namespace XLAuthenticatorNet.Models.Events;

/// <summary>
/// The account switched event args class
/// </summary>
/// <seealso cref="EventArgs"/>
internal class AccountSwitchedEventArgs : EventArgs {
  /// <summary>
  /// Gets the value of the old account
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal TotpAccount? OldAccount { get; }
  /// <summary>
  /// Gets the value of the new account
  /// </summary>
  internal TotpAccount? NewAccount { get; }
  /// <summary>
  /// Gets the value of the should switch
  /// </summary>
  internal bool ShouldSwitch => OldAccount?.Id != NewAccount?.Id;
  /// <summary>
  /// Initializes a new instance of the <see cref="AccountSwitchedEventArgs"/> class
  /// </summary>
  /// <param name="oldAccount">The old account</param>
  /// <param name="newAccount">The new account</param>
  internal AccountSwitchedEventArgs(TotpAccount? oldAccount, TotpAccount? newAccount) {
    this.OldAccount = oldAccount;
    this.NewAccount = newAccount;
  }
}