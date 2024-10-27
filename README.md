Endpoints
1. Listar todos os produtos

GET /api/products
Descrição

Retorna uma lista de todos os produtos cadastrados.
Exemplo de Requisição

GET /api/products

[
    {
        "id": uuid,
        "name": "Produto 1",
        "description": "Descrição do produto 1",
        "price": 29.99,
        "image": "url_imagem"
    },
    {
        "id": uuid,
        "name": "Produto 2",
        "description": "Descrição do produto 2",
        "price": 49.99,
        "image": "url_imagem"
    }
]

2. Consultar produto por ID

GET /api/products/{id}
Descrição

Retorna os detalhes de um produto específico.
Parâmetros de Rota

    id (inteiro, obrigatório): ID do produto.

Exemplo de Requisição

http

GET /api/products/17fed31f-b07b-44d1-9963-41ff2d13e950

    {
        "id": 17fed31f-b07b-44d1-9963-41ff2d13e950,
        "name": "Produto 1",
        "description": "Descrição do produto 1",
        "price": 29.99,
        "image": "url_imagem"
    }

3. Criar um novo produto

POST /api/products
Descrição

Adiciona um novo produto ao sistema.
Cabeçalhos

    Content-Type: application/json

Corpo da Requisição

    {
        "name": "Produto 1",
        "description": "Descrição do produto 1",
        "price": 29.99,
        "image": "url_imagem"
    }

4. Atualizar um produto

PATCH /api/products/{id}
Descrição

Atualiza as informações de um produto existente.
Parâmetros de Rota

    id (inteiro, obrigatório): ID do produto.

Cabeçalhos

    Content-Type: application/json

Corpo da Requisição
    {
        "name": "Produto 1",
        "description": "Descrição do produto 1",
        "price": 29.99,
        "image": "url_imagem"
    }

Exemplo de Resposta

    {
        "name": "Produto 1",
        "description": "Descrição do produto 1",
        "price": 29.99,
        "image": "url_imagem"
    }

5. Excluir um produto

DELETE /api/products/{id}
Descrição

Remove um produto do sistema.
Parâmetros de Rota

    id (inteiro, obrigatório): ID do produto.

Exemplo de Requisição

message: "Successful deleted" 

Código de Status HTTP

    200 OK - A solicitação foi bem-sucedida.
    201 Created - Um novo recurso foi criado com sucesso.
    400 Bad Request - A solicitação está incorreta (parâmetros ou corpo inválidos).
    404 Not Found - Produto não encontrado.
    500 Internal Server Error - Erro interno no servidor.