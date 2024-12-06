using System;
using System.Diagnostics.CodeAnalysis;
using XLAuthenticatorNet.Config;

namespace XLAuthenticatorNet.Models.Events;

/// <summary>
/// The account switched event args class
/// </summary>
/// <seealso cref="EventArgs"/>
internal sealed class AccountSwitchedEventArgs : EventArgs {
  /// <summary>
  /// Gets the value of the old account
  /// </summary>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal TOTPAccount? OldAccount { get; }

  /// <summary>
  /// Gets the value of the new account
  /// </summary>
  internal TOTPAccount? NewAccount { get; }

  /// <summary>
  /// Gets the value of the should switch
  /// </summary>
  internal bool ShouldSwitch => this.OldAccount?.Id != this.NewAccount?.Id;

  /// <summary>
  /// Initializes a new instance of the <see cref="AccountSwitchedEventArgs"/> class
  /// </summary>
  /// <param name="oldAccount">The old account</param>
  /// <param name="newAccount">The new account</param>
  internal AccountSwitchedEventArgs(TOTPAccount? oldAccount, TOTPAccount? newAccount) {
    this.OldAccount = oldAccount;
    this.NewAccount = newAccount;
  }
}
