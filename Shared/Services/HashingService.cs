using System.Security.Cryptography;
using System.Text;

namespace Shared.Services;

public static class HashingService
{
  public static string MD5Hashing(string password)
  {
    byte[] hashedBytes = MD5.HashData(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }

  public static string SHA256Hashing(string password)
  {
    var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }

  public static string HMACHashing(string password, string key)
  {
    var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
    var hashedBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(hashedBytes);
  }

  public static string PBKDF2Hashing(string password, byte[] salt)
  {
    var hashAlgo = new HashAlgorithmName("SHA256");
    var hashedBytes = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations: 10000, hashAlgo, outputLength: 32);
    return Convert.ToBase64String(hashedBytes);
  }

  public static string BCryptHashing(string password)
  {
    var workFactor = 11;
    string salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor);
    return BCrypt.Net.BCrypt.HashPassword(password, salt, enhancedEntropy: true);
  }

  public static bool BCryptVerify(string password, string hash)
  {
    return BCrypt.Net.BCrypt.Verify(password, hash, enhancedEntropy: true);
  }
}
