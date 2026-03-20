# API Itaú — Catálogo & Pedidos

> Desafio técnico: API REST em .NET 9 para gerenciamento de produtos e pedidos com Clean Architecture, validações no domínio e cobertura de testes unitários.

## Apresentação

A apresentação do projeto está disponível em [GitHub Pages](https://alealencarr.github.io/api-itau/).

A descrição do desafio está em docs/Desafio Técnico - Implementação.txt
---

## 📋 Índice

- [Sobre o projeto](#sobre-o-projeto)
- [Tecnologias](#tecnologias)
- [Arquitetura](#arquitetura)
- [Como rodar](#como-rodar)
- [Testando a API](#testando-a-api)
- [Endpoints](#endpoints)
- [Testes unitários](#testes-unitários)

---

## Sobre o projeto

API REST completa com dois domínios:

- **Produtos** — CRUD com soft delete (marcar como inativo)
- **Pedidos** — Criação com validação de produtos, máquina de estados de status e soft delete (marcar como cancelado)

O banco de dados é **EF Core InMemory** com seed automático de **10 produtos** e **5 pedidos** em estados variados ao iniciar a aplicação.

---

## Tecnologias

| Tecnologia | Uso |
|---|---|
| .NET 9 | Framework principal |
| ASP.NET Core | Web API |
| Entity Framework Core InMemory | Persistência |
| Serilog | Logs estruturados |
| Scalar + Swagger | Documentação interativa |
| xUnit | Testes unitários |
| NSubstitute | Mocks |
| FluentAssertions | Asserções |

---

## Arquitetura

```
Api.Itau/               → Controllers, Program.cs, Extensions
Application.Itau/       → Services, DTOs, Interfaces, Presenters
Domain.Itau/            → Entities, Aggregates, Repository Interfaces
Infra.Itau/             → DbContext, Repositories, EntityConfigurations, Seeder
Shared.Itau/            → ICommandResult<T>, CommandResult<T>, ToResult()
Tests.Itau/             → Testes Domain + Application
```

**Regras de dependência:**
- `Api` → `Application` → `Domain` ← `Infra`
- `Shared` é referenciado por todas as camadas
- `Domain` não conhece nenhuma outra camada

---

## Como rodar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022+ ou Rider ou VS Code

### 1. Clone o repositório

```bash
git clone https://github.com/alealencarr/api-itau.git
cd api-itau
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Rode a aplicação

```bash
cd src/Api.Itau
dotnet run
```

Ou pelo Visual Studio: selecione o perfil **`Api.Itau`** no dropdown de execução (ao lado do botão play) e pressione **F5**.

### 4. Acesse a documentação

| Interface | URL |
|---|---|
| Swagger (padrão) | https://localhost:7193/swagger |
| Scalar | https://localhost:7193/scalar |
| API base | http://localhost:5160 |

---

## Testando a API

### Opção 1 — Scalar  

Acesse `https://localhost:7193/scalar` e teste diretamente no browser.

### Opção 2 — Swagger

Acesse `https://localhost:7193/swagger`. e teste diretamente no browser.

### Opção 3 — Postman

Importe o arquivo `docs/Api.Itau.postman_collection.json`:

1. Abra o Postman
2. **File → Import → Upload Files**
3. Selecione `Api.Itau.postman_collection.json`
4. Todos os endpoints com payloads já estarão disponíveis

---

## Endpoints

### Produtos

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | `/api/produtos` | Lista produtos ativos | 200 |
| GET | `/api/produtos/{id}` | Produto por ID | 200, 404 |
| POST | `/api/produtos` | Cria produto | 201, 400 |
| PUT | `/api/produtos/{id}` | Atualiza produto | 204, 400, 404 |
| DELETE | `/api/produtos/{id}` | Soft delete | 204, 404 |

### Pedidos

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | `/api/pedidos` | Lista pedidos com itens | 200, 204 |
| GET | `/api/pedidos/{id}` | Pedido completo | 200, 404 |
| POST | `/api/pedidos` | Cria pedido | 201, 400, 404 |
| PUT | `/api/pedidos/{id}/status` | Avança status | 204, 400, 404 |
| DELETE | `/api/pedidos/{id}` | Cancela pedido | 204, 404 |

### Máquina de estados — Pedido

```
Pendente
 ├──> Processando
 │      ├──> Enviado ───> Entregue (terminal)
 │      └──> Cancelado (terminal)
 └──> Cancelado (terminal)
```

Transições inválidas retornam `400 Bad Request`.

---

## Testes unitários

### O que está coberto

| Camada | Classe | Cenários |
|---|---|---|
| Domain | `Produto` | Construtor, validações, atualização, soft delete |
| Domain | `Pedido` | Construtor, email, máquina de estados, cancelamento |
| Domain | `ItemPedido` | Construtor, captura de preço, subtotal, quantidade |
| Application | `ProdutosService` | Todos os métodos — happy path, 404, 400, 500 |
| Application | `PedidosService` | Todos os métodos + agrupamento de itens duplicados |

---
