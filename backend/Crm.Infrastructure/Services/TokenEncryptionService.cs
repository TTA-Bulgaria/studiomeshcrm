using Microsoft.AspNetCore.DataProtection;
using Crm.Application.Interfaces;

namespace Crm.Infrastructure.Services;

public class TokenEncryptionService : ITokenEncryptionService
{
    private readonly IDataProtector _protector;

    public TokenEncryptionService(IDataProtectionProvider provider)
    {
        _protector = provider.CreateProtector("AdAccount.AccessToken.v1");
    }

    public string Encrypt(string plaintext) => _protector.Protect(plaintext);

    public string Decrypt(string ciphertext)
    {
        try
        {
            return _protector.Unprotect(ciphertext);
        }
        catch
        {
            // Token was stored before encryption was introduced — return as-is
            // so existing accounts keep working until they re-authenticate
            return ciphertext;
        }
    }
}
