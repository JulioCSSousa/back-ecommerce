using System.Security.Cryptography;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

public class PixChargeModel{
    public string chave { get; set; }
    public string? solicitacaoPagador { get; set; }
    public Devedor? devedor { get; set; } = new Devedor();
    public Calendario calendario { get; set; } = new Calendario();
    public Valor valor{ get; set; } = new Valor();
}

public class Devedor
{
    public string? cpf {get; set; }
    public string? nome { get; set; }
}

public class Calendario
{
    [JsonIgnore]
    public string criacao { get; set;} = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
    public int expiracao { get; set; }
}

public class Valor{
    public string original { get; set; }
}