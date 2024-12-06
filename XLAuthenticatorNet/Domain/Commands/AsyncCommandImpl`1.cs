using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using System.Windows.Input;

namespace XLAuthenticatorNet.Domain.Commands;

/// <summary>
/// The implementation of the <see cref="ICommand" /> as an asynchronous method class with generic type <typeparamref name="TSource" />.
/// </summary>
/// <typeparam name="TSource">The type to inherit this class from.</typeparam>
/// <seealso cref="ICommand"/>
internal sealed class AsyncCommandImpl<TSource> : ICommand {
  /// <summary>
  /// The internal command to execute.
  /// </summary>
  private readonly Predicate<TSource?> _canExecute;

  /// <summary>
  /// The internal command to test if we can execute.
  /// </summary>
  private readonly Func<TSource?, Task> _execute;

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  internal AsyncCommandImpl(Func<TSource?, Task> execute) : this(execute, _ => true) {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="AsyncCommandImpl{T}"/> class
  /// </summary>
  /// <param name="execute">The execute</param>
  /// <param name="canExecute">The can execute</param>
  [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
  internal AsyncCommandImpl(Func<TSource?, Task> execute, Predicate<TSource?> canExecute) {
    this._execute = execute;
    this._canExecute = canExecute;
  }

  /// <summary>
  /// Tests if the method can be executed with the given parameter.
  /// </summary>
  /// <param name="parameter">The parameter</param>
  /// <returns><see langword="true" /> if this command can execute; otherwise <see langword="false" />.</returns>
  public bool CanExecute(object? parameter)
    => this._canExecute((TSource?)parameter);

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
  [SuppressMessage("ReSharper", "AsyncVoidMethod")]
  public async void Execute(object? parameter)
    => await this._execute((TSource?)parameter);

  /// <summary>
  /// Refreshes this instance
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void Refresh()
    => CommandManager.InvalidateRequerySuggested();
}
