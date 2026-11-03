using System.Security.Cryptography;
using System.Text;

namespace BTB.Core.Security;

/// <summary>
/// Provides encryption and decryption using the Windows Data Protection API scoped to the current user.
/// </summary>
public sealed class DpapiEncryptionService : IEncryptionService
{
    /// <summary>
    /// Encrypts the specified plain-text value and encodes the result as Base64.
    /// </summary>
    public string Encrypt(string plainText)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plainText);

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

        byte[] encryptedBytes = ProtectedData.Protect(plainBytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);

        return Convert.ToBase64String(encryptedBytes);
    }

    /// <summary>
    /// Decodes and decrypts a Base64-encoded value for the current user.
    /// </summary>
    public string? Decrypt(string? encryptedText)
    {
        if (string.IsNullOrWhiteSpace(encryptedText))
        {
            return null;
        }

        try
        {
            byte[] encryptedBytes = Convert.FromBase64String(encryptedText);

            byte[] decryptedBytes = ProtectedData.Unprotect(encryptedBytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Unable to decrypt value.", ex);
        }
    }
}