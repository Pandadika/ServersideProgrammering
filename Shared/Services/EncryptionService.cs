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

  public static string AESEncrypt(string text, string key)
  {
    if (key.Length != 16 && key.Length != 24 && key.Length != 32)
    {
      throw new ArgumentException("Key length must be 16, 24 or 32 characters.");
    }

    byte[] encryptedBytes;
    using (var aes = Aes.Create())
    {
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = new byte[16];
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
      {
        using (var ms = new MemoryStream())
        {
          using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
          {
            using (var sw = new StreamWriter(cs))
            {
              sw.Write(text);
            }
          }
          encryptedBytes = ms.ToArray();
        }
      }
    }
    return Convert.ToBase64String(encryptedBytes);
  }

  public static string AESDecrypt(string encryptedText, string key)
  {
    byte[] encryptedBytes = Convert.FromBase64String(encryptedText);
    string decryptedText;
    using (var aes = Aes.Create())
    {
      aes.Key = Encoding.UTF8.GetBytes(key);
      aes.IV = new byte[16];
      aes.Mode = CipherMode.CBC;
      aes.Padding = PaddingMode.PKCS7;

      using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
      {
        using (var ms = new MemoryStream(encryptedBytes))
        {
          using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
          {
            using (var sr = new StreamReader(cs))
            {
              decryptedText = sr.ReadToEnd();
            }
          }
        }
      }
    }
    return decryptedText;
  }
}
