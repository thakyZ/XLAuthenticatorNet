using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLAuthenticatorNet.Models.Events;
internal sealed class BooleanEventArgs : EventArgs {
  /// <summary>
  /// The value of the boolean event arguments.
  /// </summary>
  public bool Value { get; set; }

  /// <summary>
  /// Constructs a new instance of <see cref="BooleanEventArgs" />.
  /// </summary>
  /// <param name="value">The value passed to this instance.</param>
  public BooleanEventArgs(bool value) {
    this.Value = value;
  }
}
