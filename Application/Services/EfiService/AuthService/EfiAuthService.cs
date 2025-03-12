using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using System.Text;
using DotNetEnv;

public class EfiAuthService
{
    private readonly HttpClient _httpClient;

    public EfiAuthService()
    {
        Env.Load(); // Carregar variáveis de ambiente

        // Caminho do certificado P12
        string? certPath = Environment.GetEnvironmentVariable("CERT_PATH");

        if (!File.Exists(certPath))
        {
            throw new Exception($"Certificado não encontrado no caminho: {certPath}");
        }

        // Carrega o certificado com opções adicionais
        var cert = new X509Certificate2(certPath, "", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(cert);

        _httpClient = new HttpClient(handler);
    }

    public async Task<string> GetAccessTokenAsync()
    {
        try
        {
            var authUrl = "https://pix.api.efipay.com.br/oauth/token";

            string? clientId = Environment.GetEnvironmentVariable("ClientId");
            string? clientSecret = Environment.GetEnvironmentVariable("ClientSecret");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                throw new Exception("ClientId ou ClientSecret não foram carregados corretamente.");
            }

            var authorization = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}"));

            var request = new HttpRequestMessage(HttpMethod.Post, authUrl);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authorization);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var jsonBody = JsonSerializer.Serialize(new Dictionary<string, string>
            {
                { "grant_type", "client_credentials" }
            });

            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var jsonResponse = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Erro ao obter token: {response.StatusCode} - {jsonResponse}");
            }

            var tokenData = JsonSerializer.Deserialize<EfiAuthResponse>(jsonResponse);
            return tokenData?.AccessToken ?? throw new Exception("Falha ao recuperar access token.");
        }
        catch (Exception ex)
        {
            throw new Exception($"Erro na autenticação: {ex.Message}");
        }
    }
}

