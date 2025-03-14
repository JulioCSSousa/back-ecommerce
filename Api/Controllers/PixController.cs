using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class PixController : ControllerBase
{
    private readonly HttpClient _httpClient;

    [HttpPost("create-charge")]
    public async Task<IActionResult> CreateCharge([FromBody] PixChargeModel request, string token)
    {
        try
        {
            var pixService = new EfiPixService(token);
            var create = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var cpf = request.devedor.cpf?.Trim();

            if (!string.IsNullOrEmpty(cpf) && !Regex.IsMatch(request.devedor.cpf, "^[0-9]{11}$"))
            {

                return BadRequest($"CPF deve conter exatamente 11 dígitos numéricos.{request.devedor.cpf.Count()}");
            }
            // Create Pix charge
            string response = await pixService.CreatePixChargeAsync(
                request.calendario.criacao ?? create,
                request.valor.original,
                request.chave,  // Chave Pix
                request.solicitacaoPagador,  // Solicitação do pagador
                request.devedor.nome,  // Nome do devedor
                request.devedor.cpf
            );

            return Ok(new { message = "Pix charge created successfully", data = response });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("get-charges")]
    public async Task<IActionResult> GetCharges(string token, string inicio, string fim)
    {

        // Usando o token fornecido para criar o HttpClient
        var httpClient = EfiAuthService.CreateHttpClient(token);
        var url = $"https://pix.api.efipay.com.br/v2/pix?inicio={inicio}&fim={fim}";

        try
        {
            var response = await httpClient.GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Error fetching charges: {jsonResponse}");
            }

            return Ok(jsonResponse); 
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpPost("loc/create")]
    public async Task<IActionResult> CreateLoc(string token, [FromBody] LocModel model){
        var httpClient = EfiAuthService.CreateHttpClient(token);

        string url = "https://pix.api.efipay.com.br/v2/loc";
        try{
            var jsonContent = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(url, jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error creating charge: {response.StatusCode} - {jsonResponse}");
            }
            return Ok(response);
        } catch(Exception ex){
            return BadRequest(ex);
        }

    }

    [HttpGet("get-locs")]
    public async Task<IActionResult> GetLocs(string token, string inicio, string fim)
    {

        // Usando o token fornecido para criar o HttpClient
        var httpClient = EfiAuthService.CreateHttpClient(token);
        var url = $"https://pix.api.efipay.com.br/v2/loc?inicio={inicio}&fim={fim}";

        try
        {
            var response = await httpClient.GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Error fetching charges: {jsonResponse}");
            }

            return Ok(jsonResponse); 
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }

    [HttpGet("qrcode")]
    public async Task<IActionResult> GetQrCode(string token, string id)
    {

        // Usando o token fornecido para criar o HttpClient
        var httpClient = EfiAuthService.CreateHttpClient(token);
        var url = $"https://pix.api.efipay.com.br/v2/loc/{id}/qrcode";

        try
        {
            var response = await httpClient.GetAsync(url);
            var jsonResponse = await response.Content.ReadAsStringAsync();


            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"Error fetching charges: {jsonResponse}");
            }

            return Ok(jsonResponse); 
        }
        catch (Exception ex)
        {
            return BadRequest($"Error: {ex.Message}");
        }
    }
}
