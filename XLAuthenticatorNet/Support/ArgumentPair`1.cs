using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The argument pair class
/// </summary>
/// <seealso cref="ArgumentPair"/>
internal sealed class ArgumentPair<T> : ArgumentPair {
  /// <summary>
  /// Initializes a new instance of the <see cref="ArgumentPair{T}"/> class
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="value">The value</param>
  internal ArgumentPair(string name, T value) : base(name, value) {}
}
