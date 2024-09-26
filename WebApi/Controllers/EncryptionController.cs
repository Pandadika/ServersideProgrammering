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

  [HttpGet("publickey")]
  public string GetPublicKey()
  {
    return encryptionService.GetRsaPublicKey();
  }
}
