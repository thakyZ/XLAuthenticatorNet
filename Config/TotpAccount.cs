using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security;
using AdysTech.CredentialManager;
using Newtonsoft.Json;
using OtpNet;
using Serilog;
using XLAuthenticatorNet.Config.Converters;
using XLAuthenticatorNet.Extensions;

namespace XLAuthenticatorNet.Config;

/// <summary>
/// The totp account class
/// </summary>
/// <seealso cref="IEquatable{TotpAccount}"/>
/// <seealso cref="IKeyProvider"/>
[Serializable]
internal class TotpAccount : IEquatable<TotpAccount>, IKeyProvider {
  /// <summary>
  /// The credentials prefix
  /// </summary>
  private const string _CREDENTIALS_PREFIX = "XIVQuickLauncherAuth";

  /// <summary>
  /// Gets or inits the value of the id
  /// </summary>
  [JsonProperty("id"), JsonConverter(typeof(GuidJsonConverter))]
  public Guid Id { get; init; }
  /// <summary>
  /// Gets or sets the value of the name
  /// </summary>
  [JsonProperty("name")]
  public string Name { get; set; }
  /// <summary>
  /// Gets or sets the value of the launcher ip address
  /// </summary>
  [JsonProperty("address"), JsonConverter(typeof(IPAddressJsonConverter))]
  public IPAddress? LauncherIpAddress { get; set; }

  /// <summary>
  /// Gets the value of the is current
  /// </summary>
  [JsonIgnore]
  internal bool IsCurrent => App.AccountManager.CurrentAccount?.Id == Id;

  /// <summary>
  /// Gets or sets the value of the token
  /// </summary>
  [JsonIgnore]
  internal SecureString? Token {
    get {
      NetworkCredential? credentials = CredentialManager.GetCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
      return credentials?.SecurePassword;
    }
    set {
      try {
        CredentialManager.RemoveCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
      } catch (Win32Exception) {
        // ignored
      } catch (Exception ex) {
        Log.Error(ex, "Failed to remove credential from Credential Manager.");
      }

      if (value.IsNullOrEmptyOrWhiteSpace()) {
        try {
          _ = CredentialManager.RemoveCredentials($"XIVQuickLauncherAuth-{this.Id:B}");
        } catch (Exception e) {
          Log.Error(e, "Failed to remove credential from Credential Manager. (May be edge case)");
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
  /// Initializes a new instance of the <see cref="TotpAccount"/> class
  /// </summary>
  /// <param name="name">The name</param>
  /// <param name="launcherIpAddress">The launcher ip address</param>
  /// <param name="token">The token</param>
  internal TotpAccount(string name, IPAddress? launcherIpAddress, SecureString? token = null) {
    this.Name = name;
    this.Id = Guid.NewGuid();
    if (launcherIpAddress is not null) {
      this.LauncherIpAddress = launcherIpAddress;
    }

    if (token is not null) {
      this.Token = token;
    }
  }

#pragma warning disable CS8618, CS9264
  /// <summary>
  /// Initializes a new instance of the <see cref="TotpAccount"/> class
  /// </summary>
  [SuppressMessage("ReSharper", "UnusedMember.Global"),
   SuppressMessage("Compiler", "CS8618:Non-nullable variable must contain a non-null value when exiting constructor."),
   SuppressMessage("Compiler", "CS9264:Non-nullable property must contain a non-null value when exiting constructor.")]
  internal TotpAccount() {}
#pragma warning restore CS8618, CS9264

  /// <summary>
  /// Computes the hmac using the specified mode
  /// </summary>
  /// <param name="mode">The mode</param>
  /// <param name="data">The data</param>
  /// <returns>The byte array</returns>
  public byte[] ComputeHmac(OtpHashMode mode, byte[] data) => this.Token?.ToString()?.ToByteArray() ?? [];

  /// <summary>
  /// Returns the string
  /// </summary>
  /// <returns>The string</returns>
  public override string ToString() => Id.ToString();

  /// <summary>
  /// Equalses the other
  /// </summary>
  /// <param name="other">The other</param>
  /// <returns>The bool</returns>
  public bool Equals(TotpAccount? other) => other is not null && Id == other.Id;
  /// <summary>
  /// Equalses the obj
  /// </summary>
  /// <param name="obj">The obj</param>
  /// <returns>The bool</returns>
  public override bool Equals(object? obj) => obj is TotpAccount other && Equals(other);
  /// <summary>
  /// Gets the hash code
  /// </summary>
  /// <returns>The int</returns>
  public override int GetHashCode() => HashCode.Combine(Id);

  /// <summary>
  /// Creates the totp
  /// </summary>
  /// <returns>The totp</returns>
  public Totp? CreateTOTP() {
    try {
      if (this.Token?.ToPlainText() is string @string) {
        return new Totp(Base32Encoding.ToBytes(@string), mode: OtpHashMode.Sha1, step: 30, totpSize: 6, timeCorrection: new TimeCorrection(GetNistTime()));
      }
    } catch {
      // Ignore...
    }

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
      using HttpResponseMessage response = httpClient.GetAsync("https://www.google.com").Result;
      if (response is { IsSuccessStatusCode: true, Headers.Date: not null }) {
        return DateTime.ParseExact(response.Headers.Date.Value.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"),
                                   "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                                   CultureInfo.InvariantCulture.DateTimeFormat,
                                   DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal);
      }
      throw new Exception("Failed to get exact time for the TOTP");
    } catch (Exception ex) {
      throw new Exception("Failed to get exact time for the TOTP", ex);
    }
  }
}