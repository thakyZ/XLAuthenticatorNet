using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Commands;

/// <summary>
/// The command impl class
/// </summary>
/// <seealso cref="ICommand"/>
internal class CommandImpl<T> : ICommand where T : notnull {
  /// <summary>
  /// The execute
  /// </summary>
  private readonly Action<T?> _execute;
  /// <summary>
  /// The can execute
  /// </summary>
  private readonly Predicate<object?> _canExecute;

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal CommandImpl(Action<T?> execute): this(execute, (object? _) => true) { }

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal CommandImpl(Action<T?> execute, Predicate<object?> canExecute) {
    this._execute = execute;
    this._canExecute = canExecute;
  }

  /// <summary>
  /// Cans the execute using the specified parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  /// <returns>The bool</returns>
  public bool CanExecute(object? parameter) => this._canExecute(parameter);
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
  public void Execute(object? parameter) => this._execute((T?)parameter);

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void Refresh() => CommandManager.InvalidateRequerySuggested();
}