using Microsoft.AspNetCore.DataProtection;
using Shared.Services;

namespace Test;
public class UnitTest1
{
  readonly HashingService HashingService = new();

  [Fact]
  public void Test1()
  {
    var hash = HashingService.MD5Hashing("password");
    Assert.Equal("X03MO1qnZdYdgyfeuILPmQ==", hash);
  }

  [Fact]
  public void Test2()
  {
    var hash = HashingService.SHA256Hashing("password");
    Assert.Equal("XohImNooBHFR0OVvjcYpJ3NgPQ1qq73WKhHvch0VQtg=", hash);
  }

  [Fact]
  public void Test3()
  {
    var hash = HashingService.BCryptHashing("password");
    Assert.True(HashingService.BCryptVerify("password", hash));
  }

  [Fact]
  public void Test4()
  {
    var encryptionService = new EncryptionService(new EphemeralDataProtectionProvider());
    var encrypted = encryptionService.ProtectorEncrypt("password");
    var decrypted = encryptionService.ProtectorDecrypt(encrypted);

    Assert.Equal("password", decrypted);
  }

  [Fact]
  public void Test5()
  {
    var encrypted = EncryptionService.AESEncrypt("password", "keykeykeykeykey1"); //key lenght must be 16, 24 or 32 chars
    var decrypted = EncryptionService.AESDecrypt(encrypted, "keykeykeykeykey1");

    Assert.Equal("password", decrypted);
  }

  [Fact]
  public void Test6()
  {
    var encryptionService = new EncryptionService(new EphemeralDataProtectionProvider());
    var key = encryptionService.GetRsaPublicKey();
    var encrypted = encryptionService.RSAEncrypt("password", key);
    var decrypted = encryptionService.RSADecrypt(encrypted);
    Assert.Equal("password", decrypted);
  }
}