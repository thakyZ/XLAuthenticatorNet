using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Commands;

/// <summary>
/// The async command impl class
/// </summary>
/// <seealso cref="ICommand"/>
internal class AsyncCommandImpl : ICommand {
  /// <summary>
  /// The can execute
  /// </summary>
  private readonly Predicate<object?> _canExecute;
  /// <summary>
  /// The execute
  /// </summary>
  private readonly Func<Task> _execute;

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal AsyncCommandImpl(Func<Task> execute) : this(execute, _ => true) {}

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal AsyncCommandImpl(Func<Task> execute, Predicate<object?> canExecute) {
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
  [SuppressMessage("ReSharper", "AsyncVoidMethod")]
  public async void Execute(object? parameter) => await this._execute();
}