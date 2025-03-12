using Microsoft.AspNetCore.Mvc;


[Route("api/efi")]
[ApiController]
public class EfiController : ControllerBase
{
    private readonly EfiAuthService _efiAuthService;

    public EfiController(EfiAuthService efiAuthService, HttpClient httpClient)
    {
        _efiAuthService = efiAuthService;
    }

    // 🔹 Endpoint para obter o token
    [HttpGet("token")]
    public async Task<IActionResult> GetToken()
    {
        try
        {
            var token = await _efiAuthService.GetAccessTokenAsync();
            return Ok(new { access_token = token });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    

}
