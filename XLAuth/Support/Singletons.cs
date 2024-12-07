using System.Collections.Concurrent;
using System.Reflection;

namespace XLAuth.Support;

/// <summary>
/// Singletons instead of instance method.
/// </summary>
internal sealed class Singletons : IDisposable {
  /// <summary>
  /// Determines if the class is disposed.
  /// </summary>
  private bool _isDisposed;

  /// <summary>
  /// Lazy implementation of a static instance of <see cref="Singletons" />.
  /// </summary>
  private static readonly Lazy<Singletons> _lazy = new(() => new Singletons());

  /// <summary>
  /// Dictionary of static singleton objects.
  /// </summary>
  private readonly ConcurrentDictionary<Type, object> _activeInstances = new();

  /// <summary>
  /// Gets the singleton of a type.
  /// </summary>
  /// <typeparam name="TSource">The type to retrieve.</typeparam>
  /// <returns>The instance of the type.</returns>
  /// <exception cref="Exception">Thrown if a singleton of the provided type is not initialized.</exception>
  public static TSource Get<TSource>() {
    if (_lazy.Value._activeInstances.TryGetValue(typeof(TSource), out object? o) && o is not null) {
      return (TSource)o;
    }

    throw new Exception($"Singleton not initialized '{typeof(TSource).FullName}'.");
  }

  /// <summary>
  /// Checks if a singleton is registered.
  /// </summary>
  /// <typeparam name="TSource">The type to test.</typeparam>
  /// <returns><see langword="true" /> if registered and initialized; otherwise <see langword="false" />.</returns>
  public static bool IsRegistered<TSource>() {
    return _lazy.Value._activeInstances.ContainsKey(typeof(TSource));
  }

  /// <summary>
  /// Registers a new singleton of a type.
  /// </summary>
  /// <typeparam name="TSource">The type to register.</typeparam>
  /// <param name="newSingleton">An instance of the type to register.</param>
  /// <exception cref="Exception">Thrown if the instance failed to register.</exception>
  public static void Register<TSource>(TSource? newSingleton) where TSource : class {
    if (newSingleton is null) {
      newSingleton = Singletons.Register<TSource>();
      if (newSingleton is null) {
        Logger.Debug("Failed to construct singleton of for type {0}", typeof(TSource).GetType());
        return;
      }
    }

    if (!_lazy.Value._activeInstances.TryAdd(typeof(TSource), newSingleton)) {
      throw new Exception($"Failed to register new singleton for type {newSingleton.GetType()}");
    }
  }

  /// <summary>
  /// Registers a new singleton of a type. Using <see cref="System.Reflection" /> to construct an instance.
  /// </summary>
  /// <typeparam name="TSource">The type to register and construct.</typeparam>
  /// <returns>A new instance of the provided type.</returns>
  private static TSource? Register<TSource>() where TSource : class {
    if (typeof(TSource).GetConstructor([]) is not ConstructorInfo ctor) {
      return null;
    }
    return (TSource?)ctor.Invoke(parameters: null);
  }

  /// <summary>
  /// Updates an already registered singleton with a new instance.
  /// </summary>
  /// <typeparam name="TSource">The type of singleton to update.</typeparam>
  /// <param name="newSingleton">An instance of the type to update.</param>
  public static void Update<TSource>(TSource newSingleton) {
    if (newSingleton is null) {
      return;
    }

    if (_lazy.Value._activeInstances.ContainsKey(typeof(TSource))) {
      _lazy.Value._activeInstances[typeof(TSource)] = newSingleton;
    }
  }

  /// <summary>
  /// Disposes of the <see cref="Singletons" /> class.
  /// </summary>
  /// <param name="disposing">Specifies if to dispose of managed objects.</param>
  public void Dispose(bool disposing) {
    if (!_isDisposed) {
      if (disposing) {
        // NOTE: Dispose of unmanaged resources here.
        foreach (object singleton in _lazy.Value._activeInstances.Values) {
          // Only dispose the disposable objects that we own
          if (singleton is IDisposable disposable) {
            disposable.Dispose();
          }
        }

        _lazy.Value._activeInstances.Clear();
      }

      // NOTE: Dispose of managed resources here.
      _isDisposed = true;
    }
  }

  ~Singletons() {
    this.Dispose(disposing: false);
  }

  /// <inheritdoc cref="IDisposable.Dispose" />
  public void Dispose() {
    this.Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Disposes of the <see cref="Singletons" /> class.
  /// </summary>
  public static void Unload() {
    _lazy.Value.Dispose();
  }
}
