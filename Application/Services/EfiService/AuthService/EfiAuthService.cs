using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using DotNetEnv;

public class EfiAuthService
{
    private readonly HttpClient _httpClient;

    public EfiAuthService()
    {
        Env.Load(); // Carregar variáveis de ambiente

        // Carregar certificado reutilizando a função LoadCertificate
        var certWithPrivateKey = LoadCertificate();

        // Criar handler do HttpClient com o certificado
        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certWithPrivateKey);

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

    public static X509Certificate2 LoadCertificate()
    {
        string? certPath = Environment.GetEnvironmentVariable("CERT_PATH");
        string? keyPath = Environment.GetEnvironmentVariable("KEY_PATH");

        if (string.IsNullOrWhiteSpace(certPath) || string.IsNullOrWhiteSpace(keyPath))
        {
            throw new Exception("Caminhos do certificado ou chave privada não definidos nas variáveis de ambiente.");
        }

        if (!File.Exists(certPath) || !File.Exists(keyPath))
        {
            throw new Exception("Certificado ou chave privada não encontrados nos caminhos especificados.");
        }

        // Ler os arquivos .crt e .key
        string certPem = File.ReadAllText(certPath).Trim();
        string keyPem = File.ReadAllText(keyPath).Trim();

        if (!certPem.Contains("BEGIN CERTIFICATE") || !keyPem.Contains("BEGIN PRIVATE KEY"))
        {
            throw new Exception("O certificado ou a chave privada estão em formato inválido.");
        }

        // Criar certificado a partir do PEM
        using var cert = X509Certificate2.CreateFromPem(certPem);

        // Criar chave privada separadamente e combiná-la ao certificado
        using RSA privateKey = RSA.Create();
        privateKey.ImportFromPem(keyPem);
        return cert.CopyWithPrivateKey(privateKey);
    }

    public static HttpClient CreateHttpClient(string accessToken)
    {
        Env.Load();

        // Carregar o certificado reutilizando a função LoadCertificate
        var certificate = LoadCertificate();

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(certificate);

        var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

        return httpClient;
    }
}

