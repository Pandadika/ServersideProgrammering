namespace Shared.Services;

/// <summary>
/// Til symmetrisk og asymmetrist kryptering, RSA er asymmetrisk kryptering. DataProtector er symmetrisk kryptering.
/// </summary>
public interface IEncryptionService
{
  string ProtectorEncrypt(string text);
  string ProtectorDecrypt(string encryptedText);
  string RSAEncrypt(string text, string? publicKey = null);
  string RSADecrypt(string encryptedText);
  string GetRsaPublicKey();
}