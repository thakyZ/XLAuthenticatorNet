using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;

namespace XLAuth.Domain.Commands;

/// <summary>
/// The implementation of the <see cref="ICommand" /> class.
/// </summary>
/// <seealso cref="ICommand"/>
internal sealed class CommandImpl : ICommand {
  /// <summary>
  /// The internal command to execute.
  /// </summary>
  private readonly Predicate<object?> _canExecute;

  /// <summary>
  /// The internal command to test if we can execute.
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
  private CommandImpl(Action execute, Predicate<object?> canExecute) {
    this._execute = execute;
    this._canExecute = canExecute;
  }

  /// <summary>
  /// Tests if the method can be executed with the given parameter.
  /// </summary>
  /// <param name="parameter">The parameter</param>
  /// <returns><see langword="true" /> if this command can execute; otherwise <see langword="false" />.</returns>
  public bool CanExecute(object? parameter)
    => this._canExecute(parameter);

  /// <summary>
  /// The event handler when the indicator of this command's execution requirements change.
  /// </summary>
  public event EventHandler? CanExecuteChanged {
    add => CommandManager.RequerySuggested += value;
    remove => CommandManager.RequerySuggested -= value;
  }

  /// <summary>
  /// Executes the command with the given parameter
  /// </summary>
  /// <param name="parameter">The parameter</param>
  public void Execute(object? parameter)
    => this._execute();

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  internal void Refresh()
    => CommandManager.InvalidateRequerySuggested();
}
