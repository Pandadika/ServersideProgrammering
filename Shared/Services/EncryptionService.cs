using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;
using System.Text;

namespace Shared.Services;

public class EncryptionService : IEncryptionService
{
  private readonly IDataProtector dataProtector;
  private readonly string privateKey;
  private readonly string publicKey;

  private const string PrivateKeyFilePath = "/app/keys/privateKey.xml";
  private const string PublicKeyFilePath = "/app/keys/publicKey.xml";

  public EncryptionService(IDataProtectionProvider dataProtectionProvider)
  {
    dataProtector = dataProtectionProvider.CreateProtector("EncryptionService");

    if (!File.Exists(PrivateKeyFilePath) || !File.Exists(PublicKeyFilePath))
    {
      using RSACryptoServiceProvider rsa = new();

      privateKey = rsa.ToXmlString(true);
      publicKey = rsa.ToXmlString(false);
      Directory.CreateDirectory(Path.GetDirectoryName(PrivateKeyFilePath));

      File.WriteAllText(PrivateKeyFilePath, privateKey);
      File.WriteAllText(PublicKeyFilePath, publicKey);
    }
    else
    {
      privateKey = File.ReadAllText(PrivateKeyFilePath);
      publicKey = File.ReadAllText(PublicKeyFilePath);
    }
  }

  public string RSAEncrypt(string text, string? publicKey = null)
  {
    using RSACryptoServiceProvider rsa = new();
    if (publicKey == null)
    {
      rsa.FromXmlString(this.publicKey);
    }
    else
    {
      rsa.FromXmlString(publicKey);
    }
    byte[] encryptedBytes = rsa.Encrypt(Encoding.UTF8.GetBytes(text), fOAEP: false);
    return Convert.ToBase64String(encryptedBytes);
  }

  public string RSADecrypt(string encryptedText)
  {
    using RSACryptoServiceProvider rsa = new();
    rsa.FromXmlString(privateKey);
    byte[] decryptedBytes = rsa.Decrypt(Convert.FromBase64String(encryptedText), fOAEP: false);
    return Encoding.UTF8.GetString(decryptedBytes);
  }

  public string GetRsaPublicKey()
  {
    return publicKey;
  }

  public string ProtectorEncrypt(string text)
  {
    return dataProtector.Protect(text);
  }

  public string ProtectorDecrypt(string encryptedText)
  {
    return dataProtector.Unprotect(encryptedText);
  }
}
