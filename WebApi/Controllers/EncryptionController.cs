using Microsoft.AspNetCore.Mvc;
using Shared.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApi.Controllers;
[Route("api/[controller]")]
[ApiController]
public class EncryptionController : ControllerBase
{
  IEncryptionService encryptionService;
  public EncryptionController(IEncryptionService encryptionService)
  {
    this.encryptionService = encryptionService;
  }
  // GET: api/<EncryptionController>
  [HttpGet]
  public IEnumerable<string> Get()
  {
    return new string[] { "value1", "value2" };
  }

  // GET api/<EncryptionController>/5
  [HttpGet("{id}")]
  public string Get(int id)
  {
    return "value";
  }

  [HttpGet("publickey")]
  public string GetPublicKey()
  {
    return encryptionService.GetRsaPublicKey();
  }

  // POST api/<EncryptionController>
  [HttpPost]
  public string Post([FromBody] string value, string? publicKey)
  {
    return encryptionService.RSAEncrypt(value, publicKey);
  }

  // PUT api/<EncryptionController>/5
  [HttpPut("{id}")]
  public void Put(int id, [FromBody] string value)
  {
  }

  // DELETE api/<EncryptionController>/5
  [HttpDelete("{id}")]
  public void Delete(int id)
  {
  }
}
