using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Security;
using AdysTech.CredentialManager;
using Newtonsoft.Json;
using OtpNet;
using XLAuth.Config.Converters;
using XLAuth.Extensions;

namespace XLAuth.Config;

/// <summary>
/// The TOTP account class
/// </summary>
/// <seealso cref="IEquatable{TotpAccount}"/>
/// <seealso cref="IKeyProvider"/>
[Serializable]
internal sealed class TOTPAccount : IEquatable<TOTPAccount>, IKeyProvider {
  /// <summary>
  /// The credentials prefix
  /// </summary>
  private const string _CREDENTIALS_PREFIX = "XIVQuickLauncherAuth";

  /// <summary>
  /// Gets the value of the account id
  /// </summary>
  [JsonProperty("id"), JsonConverter(typeof(GuidJsonConverter))]
  public Guid Id { get; init; }
  /// <summary>
  /// Gets or sets the value of the account name
  /// </summary>
  [JsonProperty("name")]
  public string Name { get; set; } = string.Empty;
  /// <summary>
  /// Gets or sets the value of the launcher IP address
  /// </summary>
  [JsonProperty("address"), JsonConverter(typeof(IPAddressJsonConverter))]
  public IPAddress? LauncherIpAddress { get; set; }

  /// <summary>
  /// Gets the a <see langword="bool" /> indicating if this is the current account.
  /// </summary>
  [JsonIgnore]
  internal bool IsCurrent => App.AccountManager.CurrentAccount?.Id == this.Id;

  /// <summary>
  /// Gets or sets the value of the token
  /// </summary>
  [JsonIgnore]
  internal SecureString? Token {
    get {
      var credentials = CredentialManager.GetCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
      return credentials?.SecurePassword;
    }
    set {
      try {
        CredentialManager.RemoveCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
      } catch (Win32Exception) {
        // ignored
      } catch (Exception ex) {
        Logger.Error(ex, "Failed to remove credential from Credential Manager.");
      }

      if (value.IsNullOrEmptyOrWhiteSpace()) {
        try {
          _ = CredentialManager.RemoveCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
        } catch (Exception e) {
          Logger.Error(e, "Failed to remove credential from Credential Manager. (May be edge case)");
        }

        return;
      }

      CredentialManager.SaveCredentials($"{_CREDENTIALS_PREFIX}-{this.Id:B}", new NetworkCredential {
        UserName = this.Id.ToString("B"),
        SecurePassword = value,
      });
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TOTPAccount"/> class
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="launcherIpAddress">The launcher IP address</param>
  /// <param name="token">The token</param>
  internal TOTPAccount(string name, IPAddress? launcherIpAddress, SecureString? token = null) {
    this.Name = name;
    this.Id = Guid.NewGuid();
    if (launcherIpAddress is not null) {
      this.LauncherIpAddress = launcherIpAddress;
    }

    if (token is not null) {
      this.Token = token;
    }
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="TOTPAccount"/> class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor. Consider declaring it as nullable.")]
  internal TOTPAccount() {}

  /// <summary>
  /// Computes the hmac using the specified mode
  /// </summary>
  /// <param name="mode">The mode</param>
  /// <param name="data">The data</param>
  /// <returns>The byte array</returns>
  public byte[] ComputeHmac(OtpHashMode mode, byte[] data) => this.Token?.ToPlainText()?.ToByteArray() ?? [];

  /// <summary>
  /// Returns the string
  /// </summary>
  /// <returns>The string</returns>
  public override string ToString() => this.Id.ToString();

  /// <summary>
  /// Equalses the other
  /// </summary>
  /// <param name="other">The other</param>
  /// <returns>The bool</returns>
  public bool Equals(TOTPAccount? other) => other is not null && this.Id == other.Id;
  /// <summary>
  /// Equalses the obj
  /// </summary>
  /// <param name="obj">The obj</param>
  /// <returns>The bool</returns>
  public override bool Equals(object? obj) => obj is TOTPAccount other && this.Equals(other);
  /// <summary>
  /// Gets the hash code
  /// </summary>
  /// <returns>The int</returns>
  public override int GetHashCode() => HashCode.Combine(this.Id);

  /// <summary>
  /// Creates the TOTP
  /// </summary>
  /// <returns>The TOTP</returns>
  public Totp? CreateTOTP() {
    try {
      if (this.Token?.ToPlainText() is string @string) {
        var nistTime = TOTPAccount.GetNistTime();
        var timeCorrection = new TimeCorrection(nistTime);
        var bytes = Base32Encoding.ToBytes(@string);
        return new Totp(bytes, step: 30, mode: OtpHashMode.Sha1, totpSize: 6, timeCorrection);
      }
#if DEBUG
    } catch (Exception exception) {
      Logger.Verbose(exception, "Failed to generate new TOTP code.");
    }
#else
    } catch {
      // Ignore...
    }
#endif

    return null;
  }

  /// <summary>
  /// Gets the nist time
  /// </summary>
  /// <exception cref="Exception">Failed to get exact time for the TOTP</exception>
  /// <exception cref="Exception">Failed to get exact time for the TOTP </exception>
  /// <returns>The date time</returns>
  public static DateTime GetNistTime() {
    // Get UTC time from the response header of request to "https://www.google.com"
    using var httpClient = new HttpClient();
    try {
      using var response = httpClient.GetAsync("https://www.google.com").Result;
      if (response is { IsSuccessStatusCode: true, Headers.Date: not null }) {
        return response.Headers.Date.Value.DateTime;
      }
      throw new Exception("Failed to get exact time for the TOTP");
    } catch (Exception ex) {
      throw new Exception("Failed to get exact time for the TOTP", ex);
    }
  }
}
