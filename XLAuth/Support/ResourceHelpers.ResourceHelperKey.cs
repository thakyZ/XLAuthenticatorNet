namespace XLAuth.Support;

internal sealed partial class ResourceHelpers {
  /// <summary>
  /// A helper for the resource cache.
  /// </summary>
  /// <param name="key">The key identifier for the resource.</param>
  /// <param name="type">The type of the resource.</param>
  internal readonly struct ResourceHelperKey(string key, Type type) : IEquatable<ResourceHelperKey>, IEquatable<object> {
    public string Key { get; } = key;
    public Type Type { get; } = type;

    public bool Equals(ResourceHelperKey other)
      => this.Type == other.Type
      && this.Key.Equals(other.Key, StringComparison.Ordinal);

    public override bool Equals(object? obj)
      => obj is ResourceHelperKey resourceHelper
      && this.Equals(other: resourceHelper);

    public override int GetHashCode()
      => HashCode.Combine(this.Key.GetHashCode(StringComparison.Ordinal), this.Type.GetHashCode());
  }
}
