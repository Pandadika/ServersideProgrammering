namespace Shared.Services;

public interface IEncryptionService
{
  string ProtectorEncrypt(string text);
  string ProtectorDecrypt(string encryptedText);
  string RSAEncrypt(string text, string? publicKey = null);
  string RSADecrypt(string encryptedText);
  string GetRsaPublicKey();
}