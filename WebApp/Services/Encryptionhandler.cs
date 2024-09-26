namespace WebApp.Services;

public class Encryptionhandler
{
  private readonly HttpClient httpClient;
  public Encryptionhandler(HttpClient httpClient)
  {
    this.httpClient = httpClient;
  }

  public async Task<string> Encrypt(string text)
  {
    var response = await httpClient.PostAsJsonAsync("https://localhost:5001/encryption", text);
    response.EnsureSuccessStatusCode();
    return await response.Content.ReadAsStringAsync();
  }
}
