# API ECOMMERCE Lar doce Ar

Essa API contém os Endpoints do ecommerce Lar Doce Ar.

Você pode fazer GET, POST, UPDATE, DELETE dos Produtos, Usuários, e interagir com a EFI Bank API.

## 🚀 API DOC 

Acesse os endpoints no Swagger:

- **Ambiente de produção:** [https://back-ecommerce-wl58.onrender.com/swagger/index.html]

# Payment Creation Flow

Este fluxo descreve como criar um pagamento utilizando a API de Pix. Siga as etapas abaixo:

## 1. Obtenção do Token

**Request**:  
`POST /api/efi/token`

**Response**:  
O endpoint retorna uma string `Token` que será usado para qualquer requisição nos endpoints.

---

## 2. Criação da Cobrança Pix

**Request**:  
`POST /api/pix/create-charge`

**Body** (no corpo da requisição):

```json
{
  "chave": "juliosousa.dev@gmail.com",
  "solicitacaoPagador": "string",
  "devedor": {
    "cpf": "11 digits", 
    "nome": "string"
  },
  "calendario": {
    "expiracao": 3600
  },
  "valor": {
    "original": "25.00"
  }
}
```

### Response
``` json
{
  "message": "Pix charge created successfully",
  "data": "{\"calendario\":{\"criacao\":\"2025-03-14T15:56:26.352Z\",\"expiracao\":3600},\"txid\":\"bda556f19e91432f9e19dfdc2b518363\",\"revisao\":0,\"status\":\"ATIVA\",\"valor\":{\"original\":\"1.25\"},\"chave\":\"juliosousa.dev@gmail.com\",\"devedor\":{\"cpf\":\"43155140801\",\"nome\":\"Julio C S Sousa\"},\"solicitacaoPagador\":\"pix\",\"loc\":{\"id\":14,\"location\":\"qrcodespix.sejaefi.com.br/v2/b86b4b4436b84d1a994ba5c686a30fd3\",\"tipoCob\":\"cob\",\"criacao\":\"2025-03-14T15:56:26.357Z\"},\"location\":\"qrcodespix.sejaefi.com.br/v2/b86b4b4436b84d1a994ba5c686a30fd3\",\"pixCopiaECola\":\"00020101021226830014BR.GOV.BCB.PIX2561qrcodespix.sejaefi.com.br/v2/b86b4b4436b84d1a994ba5c686a30fd35204000053039865802BR5905EFISA6008SAOPAULO62070503***63043922\"}"
}

LocationId = 14
```

## 2. Criação do QRCode

```json
{
  "token": "{Token}",
  "id": "{14}"  

}

Response {
"qrcode": "...",
"linkVisualizacao":"https://pix.sejaefi.com.br/cob/pagar/b86b4b4436b84d1a994ba5c686a30fd3"}

```