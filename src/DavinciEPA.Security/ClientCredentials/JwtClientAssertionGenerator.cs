using System.Security.Claims;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using DavinciEPA.Security.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DavinciEPA.Security.ClientCredentials;

/// <summary>Creates the signed JWT client assertion used for SMART Backend Services (client-credentials) token requests.</summary>
public interface IClientAssertionGenerator
{
    string CreateSignedAssertion();
}

/// <summary>RSA-signed (RS384) client assertion generator, per the SMART Backend Services specification.</summary>
public sealed class JwtClientAssertionGenerator : IClientAssertionGenerator
{
    private readonly ClientCredentialsSettings _settings;

    public JwtClientAssertionGenerator(IOptions<ClientCredentialsSettings> options)
    {
        _settings = options.Value;
    }

    public string CreateSignedAssertion()
    {
        if (string.IsNullOrWhiteSpace(_settings.SigningKeyPem))
        {
            throw new InvalidOperationException(
                "Authentication:ClientCredentials:SigningKeyPem must be configured before a client assertion can be created.");
        }

        using var rsa = RSA.Create();
        rsa.ImportFromPem(_settings.SigningKeyPem);

        var securityKey = new RsaSecurityKey(rsa) { KeyId = _settings.SigningKeyId };
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha384);

        var now = DateTime.UtcNow;
        var token = new JwtSecurityToken(
            issuer: _settings.ClientId,
            audience: _settings.Audience,
            claims: new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _settings.ClientId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            },
            notBefore: now,
            expires: now.AddSeconds(_settings.AssertionLifetimeSeconds),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
