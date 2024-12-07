using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace XLAuth.Support;

/// <summary>
/// The resource helpers class
/// </summary>
internal sealed partial class ResourceHelpers : IDisposable {
  /// <summary>
  /// Gets a resource from the assembly with the specified type parameter and using the specified resource name
  /// </summary>
  /// <typeparam name="TOut">The type to parse the resource into.</typeparam>
  /// <param name="resourceName">The resource name to fetch.</param>
  /// <returns>The fetched resource or null if none was found.</returns>
  /// <exception cref="NotImplementedException">Throws if the type of <typeparamref name="TOut"/> is not yet implemented.</exception>
  internal static TOut? GetFromResources<TOut>(string resourceName) where TOut : class {
    Type          type   = typeof(TOut);
    var           asm    = Assembly.GetExecutingAssembly();
    Stream? stream = asm.GetManifestResourceStream(resourceName);
    if (stream is null) {
      Util.ShowError($"Failed to load resource \"{resourceName}\".", "Failed to load resource");
      return null;
    }

    if (type == typeof(string)) {
      using Stream? stream1 = stream;
      using var reader = new StreamReader(stream1);
      string output = reader.ReadToEnd();
      return Singletons.Get<ResourceHelpers>()._cache.GetOrAdd(new(resourceName, type), (_, value) => value, output) as TOut
          ?? throw new InvalidOperationException("Failed to get or add cached blank bitmap image.");
    }

    if (type == typeof(Stream)) {
      return stream as TOut
          ?? throw new InvalidOperationException("Failed to get pass manifest resource stream back.");
    }

    throw new NotSupportedException($"Output type, {type.FullName} for method {nameof(GetFromResources)} is not yet implemented.");
  }

  /// <summary>
  /// Loads the app's icon from embedded resources.
  /// </summary>
  /// <returns>The app's icon as an image source.</returns>
  internal static BitmapFrame? LoadAppIcon() {
    using Stream? stream = ResourceHelpers.GetFromResources<Stream>("xlauth_icon");
    if (stream is null) {
      return null;
    }

    var decoder = new IconBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnDemand);
    return decoder.Frames[0];
  }

  /// <summary>
  /// Creates a blank bitmap source in case a resource fails to load.
  /// </summary>
  /// <returns>A blank bitmap image.</returns>
  internal static BitmapSource GetBlankBitmap() {
    using var bitmap = new Bitmap(1, 1, PixelFormat.Alpha);
    var transparent = Color.FromArgb(0, 0, 0, 0);
    bitmap.SetPixel(0, 0, transparent);
    var bitmapHandle = bitmap.GetHbitmap();
    var emptyOptions = BitmapSizeOptions.FromEmptyOptions();
    BitmapSource output = Imaging.CreateBitmapSourceFromHBitmap(bitmapHandle, nint.Zero, Int32Rect.Empty, emptyOptions);
    return Singletons.Get<ResourceHelpers>()._cache.GetOrAdd(new("blank_bitmap", typeof(BitmapSource)), (_, value) => value, output) as BitmapSource
        ?? throw new InvalidOperationException("Failed to get or add cached blank bitmap image.");
  }

#region Internal Members
#region Private Fields
  /// <summary>
  /// Specifies if this class has been disposed of or not.
  /// </summary>
  private bool _isDisposed;
  #endregion Private Fields
  #region Private Properties
  /// <summary>
  /// Gets the concurrent dictionary for caching resources.
  /// </summary>
  private readonly ConcurrentDictionary<ResourceHelperKey, object?> _cache = new();
#endregion Private Properties

#region IDisposable Implementations
  /// <summary>
  /// Disposes of managed and unmanaged resources.
  /// </summary>
  /// <param name="disposing">Specifies if the unmanaged resources should be disposed.</param>
  private void Dispose(bool disposing) {
    if (!this._isDisposed) {
      if (disposing) {
        // NOTE: Dispose of unmanaged resources here.
        foreach (object? value in this._cache.Values) {
          // Only dispose the disposable objects that we own
          if (value is IDisposable disposable) {
            disposable.Dispose();
          }
        }
        this._cache.Clear();
      }

      // NOTE: Dispose of managed resources here.
      this._isDisposed = true;
    }
  }

  /// <inheritdoc cref="IDisposable.Dispose()"/>
  public void Dispose() {
    this.Dispose(disposing: true);
    GC.SuppressFinalize(this);
  }

  ~ResourceHelpers() {
    this.Dispose(disposing: false);
  }

#endregion IDisposable Implementations
#endregion Internal Members
}
