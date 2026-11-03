namespace BTB.Core.Security;

/// <summary>
/// Defines services for encrypting and decrypting text.
/// </summary>
public interface IEncryptionService
{
    /// <summary>
    /// Encrypts the specified plain text.
    /// </summary>
    string Encrypt(string plainText);

    /// <summary>
    /// Decrypts the specified encrypted text.
    /// </summary>
    string? Decrypt(string? encryptedText);
}