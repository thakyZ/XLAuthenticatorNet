using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Commands;

/// <summary>
/// The async command impl class
/// </summary>
/// <seealso cref="ICommand"/>
[SuppressMessage("ReSharper", "UnusedType.Global")]
internal class AsyncCommandImpl<T> : ICommand {
  /// <summary>
  /// The can execute
  /// </summary>
  private readonly Predicate<T?> _canExecute;
  /// <summary>
  /// The execute
  /// </summary>
  private readonly Func<T?, Task> _execute;

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal AsyncCommandImpl(Func<T?, Task> execute) : this(execute, _ => true) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal AsyncCommandImpl(Func<T?, Task> execute, Predicate<T?> canExecute) {
    this._execute = execute;
    this._canExecute = canExecute;
  }

  /// <summary>
  /// Cans the execute using the specified parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  /// <returns>The bool</returns>
  public bool CanExecute(object? parameter) => this._canExecute((T?)parameter);

  /// <summary>
  /// The can execute changed
  /// </summary>
  public event EventHandler? CanExecuteChanged {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  /// <summary>
  /// Executes the parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  [SuppressMessage("ReSharper", "AsyncVoidMethod")]
  public async void Execute(object? parameter) => await this._execute((T?)parameter);

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void Refresh() => CommandManager.InvalidateRequerySuggested();
}