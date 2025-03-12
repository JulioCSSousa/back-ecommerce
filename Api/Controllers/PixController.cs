using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

[ApiController]
[Route("api/[controller]")]
public class PixController : ControllerBase
{

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
}
