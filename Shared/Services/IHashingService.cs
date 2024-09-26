namespace Shared.Services;

public interface IHashingService
{
  string BCryptHashing(string password);
  bool BCryptVerify(string password, string hash);
  string HMACHashing(string password, string key);
  string MD5Hashing(string password);
  string PBKDF2Hashing(string password, byte[] salt);
  string SHA256Hashing(string password);
}