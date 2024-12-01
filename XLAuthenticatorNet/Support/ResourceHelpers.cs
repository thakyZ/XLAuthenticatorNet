using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace XLAuthenticatorNet.Support;

/// <summary>
/// The resource helpers class
/// </summary>
internal sealed class ResourceHelpers : IDisposable {
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
      return _instance.Cache.GetOrAdd(resourceName, (_, value) => value, (output, type)).Value as TOut
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
    var bitmap = new Bitmap(1, 1, PixelFormat.Alpha);
    var transparent = Color.FromArgb(0, 0, 0, 0);
    bitmap.SetPixel(0, 0, transparent);
    var bitmapHandle = bitmap.GetHbitmap();
    var emptyOptions = BitmapSizeOptions.FromEmptyOptions();
    BitmapSource output = Imaging.CreateBitmapSourceFromHBitmap(bitmapHandle, nint.Zero, Int32Rect.Empty, emptyOptions);
    return _instance.Cache.GetOrAdd("blank_bitmap", (_, value) => value, (output, output.GetType())).Value as BitmapSource
        ?? throw new InvalidOperationException("Failed to get or add cached blank bitmap image.");
  }

#region Internal Members
#region Private Fields
  /// <summary>
  /// Specifies an instance of this class.
  /// </summary>
  private static ResourceHelpers _instance = null!;
  /// <summary>
  /// Specifies if this class has been disposed of or not.
  /// </summary>
  private bool _isDisposed;
#endregion Private Fields
#region Private Properties
  /// <summary>
  /// Gets the concurrent dictionary for caching resources.
  /// </summary>
  [SuppressMessage("Compiler", "CA1822:Mark member as static")]
  private ConcurrentDictionary<string, (object? Value, Type Type)> Cache => [];
#endregion Private Properties

#region Initializer
  [SuppressMessage("ReSharper", "MemberCanBeMadeStatic.Global"),
   SuppressMessage("Performance", "CA1822:Mark members as static")]
  internal void Init() {
    _instance = new ResourceHelpers();
  }
#endregion Initializer

#region IDisposable Implementations
  /// <summary>
  /// Disposes of managed and unmanaged resources.
  /// </summary>
  /// <param name="disposing">Specifies if the unmanaged resources should be disposed.</param>
  private void Dispose(bool disposing) {
    if (!this._isDisposed) {
      if (disposing) {
        // NOTE: Dispose of unmanaged resources here.
      }

      this.Cache.Clear();
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

  /// <summary>
  /// Unloads (disposes) of the instance of <see cref="ResourceHelpers"/>.
  /// </summary>
  [SuppressMessage("ReSharper", "ConditionalAccessQualifierIsNonNullableAccordingToAPIContract")]
  internal static void Unload() {
    // Null check just in case if this class was never initiated.
    _instance?.Dispose();
  }
  #endregion IDisposable Implementations
  #endregion Internal Members
}