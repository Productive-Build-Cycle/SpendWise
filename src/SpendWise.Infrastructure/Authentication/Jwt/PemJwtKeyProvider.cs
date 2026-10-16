using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace SpendWise.Infrastructure.Authentication.Jwt;

public sealed class PemJwtKeyProvider : IJwtKeyProvider, IDisposable
{
    private readonly RSA _signingRsa = RSA.Create();
    private readonly List<RSA> _validationRsaInstances = [];

    public SecurityKey SigningKey { get; }

    public IReadOnlyCollection<SecurityKey> ValidationKeys { get; }

    public PemJwtKeyProvider(JwtOptions options)
    {
        options.Validate();

        try
        {
            _signingRsa.ImportFromPem(File.ReadAllText(options.PrivateKeyPath));

            EnsureMinimumKeySize(_signingRsa.KeySize);

            var validationKeys = new List<SecurityKey>();
            RSA? activePublicRsa = null;

            foreach (var keyOptions in options.ValidationKeys)
            {
                var rsa = RSA.Create();

                try
                {
                    rsa.ImportFromPem(File.ReadAllText(keyOptions.PublicKeyPath));
                    EnsureMinimumKeySize(rsa.KeySize);
                }
                catch
                {
                    rsa.Dispose();
                    throw;
                }

                _validationRsaInstances.Add(rsa);

                validationKeys.Add(new RsaSecurityKey(rsa)
                {
                    KeyId = keyOptions.KeyId
                });

                if (string.Equals(keyOptions.KeyId, options.ActiveKeyId, StringComparison.Ordinal))
                {
                    activePublicRsa = rsa;
                }
            }

            if (activePublicRsa is null)
                throw new InvalidOperationException("The active JWT public key was not loaded.");

            var privateKeyPublicPart = _signingRsa.ExportSubjectPublicKeyInfo();

            var configuredPublicPart = activePublicRsa.ExportSubjectPublicKeyInfo();

            if (!CryptographicOperations.FixedTimeEquals(privateKeyPublicPart, configuredPublicPart))
            {
                throw new InvalidOperationException("The active JWT public and private keys do not match.");
            }

            SigningKey = new RsaSecurityKey(_signingRsa)
            {
                KeyId = options.ActiveKeyId
            };

            ValidationKeys = validationKeys.AsReadOnly();
        }
        catch
        {
            _signingRsa.Dispose();

            foreach (var rsa in _validationRsaInstances)
                rsa.Dispose();

            throw;
        }
    }

    private static void EnsureMinimumKeySize(int keySize)
    {
        if (keySize < 2048)
        {
            throw new InvalidOperationException("JWT RSA keys must be at least 2048 bits.");
        }
    }

    public void Dispose()
    {
        _signingRsa.Dispose();

        foreach (var rsa in _validationRsaInstances)
            rsa.Dispose();
    }
}
