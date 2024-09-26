using Shared.Services;
using WebApp.Models;

namespace WebApp.Services;

public class UserHandler
{
  private readonly IHttpClientFactory httpClientFactory;
  private readonly IHashingService hashingService;

  public UserHandler(IHttpClientFactory httpClientFactory, IHashingService hashingService)
  {
    this.httpClientFactory = httpClientFactory;
    this.hashingService = hashingService;
  }

  public async Task<bool> RegisterNewUser(string email)
  {
    var httpClient = httpClientFactory.CreateClient("DefaultClient");
    var newUser = new User { Email = email };
    var response = await httpClient.PostAsJsonAsync("api/users", newUser);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadAsStringAsync();
    return result == "true";
  }

  public async Task<User?> GetUser(string email)
  {
    var httpClient = httpClientFactory.CreateClient("DefaultClient");
    var response = await httpClient.GetAsync($"api/users/{email}");
    if (response.IsSuccessStatusCode)
    {
      return await response.Content.ReadFromJsonAsync<User>();
    }
    return null;
  }

  public async Task<bool> CheckCpr(string email, string cpr)
  {
    var httpClient = httpClientFactory.CreateClient("DefaultClient");
    var user = new User { Email = email, CPR = hashingService.SHA256Hashing(cpr) };
    var response = await httpClient.PutAsJsonAsync($"api/users/cpr/{email}", user);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadAsStringAsync();
    return bool.TryParse(result, out bool isSuccess) && isSuccess;
  }
}
