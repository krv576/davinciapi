using System.Security.Cryptography;
using System.Text;

namespace DavinciEPA.Security.SmartOnFhir;

/// <summary>Generates PKCE (RFC 7636) code verifier/challenge pairs for the SMART App Launch authorization code flow.</summary>
public static class PkceHelper
{
    /// <summary>Generates a cryptographically random code verifier, 43-128 characters per RFC 7636.</summary>
    public static string GenerateCodeVerifier()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Base64UrlEncode(bytes);
    }

    /// <summary>Derives the S256 code challenge for a given code verifier.</summary>
    public static string DeriveCodeChallenge(string codeVerifier)
    {
        var hash = SHA256.HashData(Encoding.ASCII.GetBytes(codeVerifier));
        return Base64UrlEncode(hash);
    }

    private static string Base64UrlEncode(byte[] bytes) =>
        Convert.ToBase64String(bytes)
            .TrimEnd('=')
            .Replace('+', '-')
            .Replace('/', '_');
}
