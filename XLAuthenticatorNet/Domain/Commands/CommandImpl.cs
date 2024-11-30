using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Commands;

/// <summary>
/// The command impl class
/// </summary>
/// <seealso cref="ICommand"/>
internal class CommandImpl : ICommand {
  /// <summary>
  /// The can execute
  /// </summary>
  private readonly Predicate<object?> _canExecute;
  /// <summary>
  /// The execute
  /// </summary>
  private readonly Action _execute;

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal CommandImpl(Action execute): this(execute, _ => true) { }

  /// <summary>
  /// Initializes a new instance of the <see cref="CommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal CommandImpl(Action execute, Predicate<object?> canExecute) {
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
  /// Executes the parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  public void Execute(object? parameter) => this._execute();

  /// <summary>
  /// The can execute changed
  /// </summary>
  public event EventHandler? CanExecuteChanged {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void Refresh() => CommandManager.InvalidateRequerySuggested();
}