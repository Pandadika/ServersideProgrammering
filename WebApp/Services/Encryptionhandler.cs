namespace WebApp.Services;

public class Encryptionhandler
{
  private readonly IHttpClientFactory httpClientFactory;
  public Encryptionhandler(IHttpClientFactory httpClientFactory)
  {
    this.httpClientFactory = httpClientFactory;
  }

  public async Task<string> GetApiKey()
  {
    var httpClient = httpClientFactory.CreateClient("DefaultClient");
    var response = await httpClient.GetAsync("api/encryption/publickey");
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
  }
}
