using System.Net.Http.Headers;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using DotNetEnv;

public class EfiPixService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiUrl = "https://pix.api.efipay.com.br/v2/cob";

    public EfiPixService(string accessToken)
    {
        Env.Load();

        // Carregar o certificado reutilizando a função LoadCertificate
        var certificate = EfiAuthService.LoadCertificate();

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);

        _httpClient = new HttpClient(handler);
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<string> CreatePixChargeAsync(
        string create,
        string value, 
        string pixKey, 
        string? payerRequest,
        string? payerName, 
        string? payerCpf)
    {
        try
        {
            // Definindo o corpo da requisição
            var requestBody = new PixChargeModel
            {
                chave = pixKey,
                solicitacaoPagador = payerRequest,
                devedor = new Devedor
                {
                    cpf = payerCpf,
                    nome = payerName
                },
                calendario = new Calendario
                {
                    criacao = create,
                    expiracao = 3600 // 1 hora de expiração
                },
                valor = new Valor
                {
                    original = value
                },
            };

            var jsonContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(_apiUrl, jsonContent);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error creating charge: {response.StatusCode} - {jsonResponse}");
            }

            return jsonResponse;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error creating Pix charge: {ex.Message}");
        }
    }
}
