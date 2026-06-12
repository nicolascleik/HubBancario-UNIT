# BankingHub — Hub de Integração Bancária Open Finance

> Plataforma B2B de integração bancária desenvolvida para a **Receba Digital**, centralizando operações Pix (COB e COBV) dentro do ecossistema Open Banking/Open Finance brasileiro.

---

## Sumário

1. [Sobre o Projeto](#sobre-o-projeto)
2. [Contexto e Origem](#contexto-e-origem)
3. [Glossário do Domínio (DDD)](#glossário-do-domínio-ddd)
4. [Arquitetura](#arquitetura)
5. [Estrutura de Pastas](#estrutura-de-pastas)
6. [Tecnologias Utilizadas](#tecnologias-utilizadas)
7. [Funcionalidades Implementadas](#funcionalidades-implementadas)
8. [Referência Completa da API](#referência-completa-da-api)
9. [Fluxo de Negócio Completo](#fluxo-de-negócio-completo)
10. [Integração Bancária](#integração-bancária)
11. [Segurança](#segurança)
12. [Ambiente Docker — Executando o Projeto](#ambiente-docker--executando-o-projeto)
13. [Guia de Testes End-to-End (E2E)](#guia-de-testes-end-to-end-e2e)
14. [Mock Itaú (WireMock)](#mock-itaú-wiremock)
15. [Testes Automatizados](#testes-automatizados)
16. [Evolução do Projeto — Do Modelo Antigo ao Novo](#evolução-do-projeto--do-modelo-antigo-ao-novo)
17. [Roadmap e Desenvolvimento Futuro](#roadmap-e-desenvolvimento-futuro)
18. [Princípios de Negócio](#princípios-de-negócio)
19. [Informações Institucionais e Equipe](#informações-institucionais-e-equipe)
20. [Status do Projeto](#status-do-projeto)

---

## Sobre o Projeto

O **BankingHub** é uma API REST B2B concebida para servir como o único ponto de integração entre a plataforma **Receba Digital** e as instituições financeiras parceiras. Antes de sua criação, cada integração bancária era tratada individualmente, gerando duplicidade de código, riscos de segurança e dificuldade de manutenção.

O Hub resolve esse problema ao assumir inteiramente a responsabilidade de:

- **Gerenciar a identidade dos clientes B2B** (contas de lojistas/ERPs)
- **Armazenar e proteger as credenciais bancárias** (ClientSecret, certificados mTLS)
- **Orquestrar o ciclo completo de cobrança Pix** — da intenção de cobrança até a confirmação de pagamento
- **Manter auditoria completa e imutável** de todas as operações
- **Processar notificações bancárias de forma assíncrona** via fila (RabbitMQ)
- **Executar conciliação automática** como mecanismo de fallback

O projeto foi concebido seguindo o princípio **API First**, permitindo que qualquer aplicação consuma serviços bancários por meio de uma interface única, padronizada e desacoplada das implementações específicas de cada banco. A arquitetura foi projetada para minimizar lock-in tecnológico, garantir auditabilidade completa e permitir a inclusão de novos bancos sem impacto nas regras centrais de negócio.

---

## Contexto e Origem

### O Cenário Anterior

O sistema anterior operava com responsabilidades distribuídas: a Receba Digital tratava cada integração bancária de forma separada, sem uma camada de abstração unificada. Isso criava fragilidade operacional e dificultava a adição de novos parceiros bancários.

### O Novo Modelo

Com o HubBancário, o Hub deixou de ser apenas um "roteador" de mensagens e passou a ser **o dono da regra de negócio bancária**. As principais mudanças foram:

| Antes | Depois |
|---|---|
| Integrações distribuídas por serviço | Centralizadas no Hub |
| Sem banco de dados próprio do Hub | PostgreSQL próprio com todas as entidades |
| Sem gerenciamento de identidade | Account + ClientSecret gerenciados pelo Hub |
| NGINX como camada de rede | Removido — comunicação direta simplificada |
| Sem fila de mensagens | RabbitMQ para webhooks assíncronos |
| Sem conciliação automática | Polling job via Hangfire como fallback |

---

## Glossário do Domínio (DDD)

Este projeto utiliza Domain-Driven Design. O glossário abaixo define os termos do domínio que permeiam todo o código, documentação e comunicação da equipe.

| Termo | Definição |
|---|---|
| **Account** | Representa a conta bancária de um lojista/ERP cadastrado no Hub. Contém os dados bancários (banco, agência, conta, CPF/CNPJ) e aponta para as credenciais de autenticação. |
| **ClientSecret** | Conjunto de credenciais de segurança exigido pelo banco parceiro para comunicação segura. Contém o `SecretValue` (client_id do banco), o `Certificate` (certificado mTLS em Base64) e a `CertificatePassword`. Uma credencial revogada nunca pode ser reativada. |
| **Invoice** | Documento financeiro (fatura) que representa a intenção de cobrança gerada pelo ERP do lojista. É o pré-requisito para a criação de uma cobrança Pix. |
| **PixCharge** | Cobrança Pix registrada no banco parceiro (Itaú). Contém o `TxId` (identificador único do Banco Central), o payload EMV (QR Code "Copia e Cola") e o status atual (`Active`, `Paid`, `Expired`). |
| **PixKey** | Chave Pix (CPF, e-mail, telefone ou aleatória) vinculada a uma Account do Hub, habilitando o recebimento de pagamentos. |
| **TxId** | Identificador único de transação Pix gerado pelo Hub seguindo o padrão do Banco Central (até 35 caracteres alfanuméricos). É a chave de conciliação universal entre todos os sistemas. |
| **COB** | Cobrança Pix imediata — sem data de vencimento, expira após um período configurável (ex: 1 hora). |
| **COBV** | Cobrança Pix com vencimento — possui data e hora de expiração definidas. |
| **EMV** | Payload padrão do QR Code Pix (string "Copia e Cola") no formato EMV definido pelo Banco Central do Brasil. |
| **Webhook** | Notificação HTTP assíncrona enviada pelo banco parceiro ao Hub informando que um pagamento foi processado. |
| **Adapter** | Implementação específica de integração com um banco (ex: `ItauPixAdapter`). Isola as particularidades de autenticação e payload de cada instituição. |
| **AuditLog** | Registro imutável de cada operação sensível realizada no sistema (criação, mudança de status, recepção de webhook). |
| **BankId** | Código COMPE da instituição financeira (ex: `"341"` para o Itaú). Utilizado pelo `BankAdapterFactory` para selecionar o Adapter correto. |
| **SecretId** | Identificador (Guid) da credencial `ClientSecret`, referenciado pela `Account` como chave estrangeira. |
| **Conciliação** | Processo de verificação ativa do status de um pagamento junto ao banco parceiro, executado tanto via webhook (reativo) quanto via Polling (proativo/fallback). |

---

## Arquitetura

O projeto segue **Clean Architecture** com **Domain-Driven Design (DDD)** e **CQRS**, organizados em quatro camadas concêntricas com dependência sempre apontando para dentro.

```
┌─────────────────────────────────────────────┐
│              API (Apresentação)             │
│   Controllers · Middlewares · Filters       │
├─────────────────────────────────────────────┤
│           Application (Casos de Uso)        │
│   Commands · Queries · Handlers · DTOs      │
├─────────────────────────────────────────────┤
│             Domain (Negócio Puro)           │
│   Aggregates · Value Objects · Exceptions   │
├─────────────────────────────────────────────┤
│          Infrastructure (Detalhes)          │
│   EF Core · RabbitMQ · Adapters · Hangfire  │
└─────────────────────────────────────────────┘
```

### Padrões Arquiteturais Aplicados

| Padrão | Aplicação no Projeto |
|---|---|
| **Clean Architecture** | Separação rígida de responsabilidades entre as 4 camadas |
| **DDD** | Aggregates (`Account`, `PixCharge`, `Invoice`), Value Objects (`Money`, `Document`, `TxId`), Factory Methods e Domain Exceptions |
| **CQRS** | Commands (mutações) e Queries (leituras) separados via MediatR |
| **Repository Pattern** | Interface por aggregate na camada Domain, implementação na Infrastructure |
| **Unit of Work** | `IUnitOfWork` garante atomicidade das transações com o banco |
| **Adapter Pattern** | `IBankPixAdapter` abstrai cada banco; `ItauPixAdapter` é a primeira implementação |
| **Factory Pattern** | `BankAdapterFactory` seleciona o Adapter correto pelo `BankId` |
| **Strategy Pattern** | Estratégia de conciliação (webhook vs. polling) desacoplada |
| **Pipeline Behavior** | `ValidationBehavior`, `LoggingBehavior` e `TransactionBehavior` no pipeline do MediatR |

### Fluxo de uma Requisição

```
ERP/Cliente HTTP
      │
      ▼
[IdempotencyFilter] ──► Bloqueia duplicatas pelo cabeçalho Idempotency-Key
      │
      ▼
[ValidationFilter] ──► Formata erros de ModelState em RFC 7807
      │
      ▼
[Controller] ──► Monta o Command/Query e envia ao MediatR
      │
      ▼
[MediatR Pipeline]
  ├── ValidationBehavior ──► Executa validators do FluentValidation
  ├── LoggingBehavior    ──► Loga entrada/saída com o CorrelationId
  ├── TransactionBehavior ──► Abre/fecha transação DB automaticamente
  └── Handler ──► Executa o caso de uso (acessa Repositories, chama Adapters)
      │
      ▼
[Repository / UnitOfWork] ──► Persiste no PostgreSQL via EF Core
      │
      ▼
HTTP Response padronizada (201/200/204/400/404/409)
```

---

## Estrutura de Pastas

```
HubBancario-UNIT/
│
├── HubBancario/
│   ├── API/                                   # Camada de Apresentação
│   │   ├── Controllers/v1/
│   │   │   ├── AccountsController.cs
│   │   │   ├── ClientCredentialsController.cs
│   │   │   ├── InvoicesController.cs
│   │   │   ├── PixChargesController.cs
│   │   │   ├── PixKeysController.cs
│   │   │   └── WebhooksController.cs
│   │   ├── Middleware/
│   │   │   ├── ExceptionMiddleware.cs         # Captura exceções → RFC 7807
│   │   │   ├── CorrelationIdMiddleware.cs     # Gera ID único por requisição
│   │   │   └── RequestLoggingMiddleware.cs    # Loga tempo e rota
│   │   ├── Filters/
│   │   │   ├── IdempotencyFilter.cs           # Anti-duplo clique
│   │   │   └── ValidationFilter.cs            # Formata erros de ModelState
│   │   ├── Extensions/
│   │   │   ├── ServiceCollectionExtensions.cs # Registro do DI
│   │   │   └── WebApplicationExtensions.cs    # Pipeline HTTP
│   │   ├── OpenApi/
│   │   │   └── hub-bancario-api.yaml          # Contrato OpenAPI
│   │   ├── appsettings.json
│   │   ├── appsettings.Development.json
│   │   └── Program.cs                         # Entry point
│   │
│   ├── Application/                           # Casos de Uso (CQRS)
│   │   ├── Behaviors/
│   │   │   ├── ValidationBehavior.cs
│   │   │   ├── LoggingBehavior.cs
│   │   │   └── TransactionBehavior.cs
│   │   ├── Commands/
│   │   │   ├── Account/                       # CreateAccount, UpdateAccount,
│   │   │   │                                  # DeleteAccount, ChangeAccountStatus
│   │   │   ├── ClientSecret/                  # CreateClientSecret, RevokeClientSecret,
│   │   │   │                                  # UpdateClientSecret
│   │   │   ├── CreateInvoice/
│   │   │   ├── CreatePixCharge/
│   │   │   ├── PixKey/                        # CreatePixKey, DeletePixKey, UpdatePixKey
│   │   │   └── ProcessWebhook/
│   │   ├── Queries/
│   │   │   ├── Account/                       # GetAccountById
│   │   │   ├── ClientCredential/              # GetClientCredentialById
│   │   │   ├── GetInvoice/
│   │   │   ├── GetPixChargeStatus/
│   │   │   └── PixKey/                        # GetPixKeyInfo, GetPixKeysByAccountId
│   │   ├── DTOs/                              # AccountDto, InvoiceDto, QrCodeResponseDto...
│   │   ├── Interfaces/                        # IBankPixAdapter, IBankAdapterFactory,
│   │   │                                      # IMessageQueue, INotificationService
│   │   ├── Mappings/
│   │   │   └── DomainToDtoMappingProfile.cs
│   │   └── DependencyInjection.cs
│   │
│   ├── Domain/                                # Regras de Negócio Puras
│   │   ├── Aggregates/
│   │   │   ├── Account/
│   │   │   │   ├── Account.cs                 # Aggregate Root
│   │   │   │   ├── ClientSecret.cs
│   │   │   │   └── PixKey.cs
│   │   │   ├── Invoice/
│   │   │   │   └── Invoice.cs
│   │   │   └── PixCharge/
│   │   │       └── PixCharge.cs
│   │   ├── ValueObjects/
│   │   │   ├── Money.cs                       # Valor monetário em BRL
│   │   │   ├── Document.cs                    # CPF/CNPJ com validação matemática
│   │   │   └── TxId.cs                        # Identificador Pix (padrão BACEN)
│   │   ├── Repositories/                      # Interfaces dos Repositories
│   │   └── Exceptions/
│   │       └── DomainException.cs
│   │
│   └── Infrastructure/                        # Detalhes Técnicos
│       ├── Persistence/
│       │   ├── BankingDbContext.cs
│       │   ├── Configurations/               # EF Core Fluent API por entidade
│       │   ├── Repositories/                 # Implementações dos Repositories
│       │   └── UnitOfWork.cs
│       ├── BankAdapters/
│       │   ├── Abstractions/
│       │   │   ├── BankAdapterFactory.cs
│       │   │   └── BaseBankPixAdapter.cs
│       │   └── Itau/
│       │       ├── ItauPixAdapter.cs
│       │       ├── ItauTokenProvider.cs
│       │       ├── ItauStatusMapper.cs
│       │       └── ItauOptions.cs
│       ├── Messaging/
│       │   ├── MessageQueueService.cs
│       │   └── RabbitMQ/
│       │       ├── RabbitMQConnection.cs
│       │       ├── RabbitMQPublisher.cs
│       │       └── WebhookConsumerWorker.cs   # BackgroundService consumidor
│       ├── BackgroundJobs/
│       │   ├── PollingJob.cs                  # Conciliação ativa (fallback)
│       │   └── ReconciliationJob.cs
│       ├── Notifications/
│       │   └── WebhookNotificationService.cs  # Notifica o ERP do cliente
│       └── DependencyInjection.cs
│
├── docker/
│   ├── docker-compose.yml
│   ├── Dockerfile
│   └── wiremock/
│       └── mappings/                          # 8 cenários de mock do Itaú
│
└── HubBancario.slnx
```

---

## Tecnologias Utilizadas

| Tecnologia | Versão | Finalidade |
|---|---|---|
| **.NET** | 10 | Plataforma principal |
| **ASP.NET Core** | 10 | API REST |
| **Entity Framework Core** | 9+ | ORM / Persistência |
| **PostgreSQL** | 16 | Banco de dados relacional |
| **MediatR** | 12.4 | Implementação de CQRS e Pipeline Behaviors |
| **FluentValidation** | 11.9 | Validação declarativa de comandos |
| **AutoMapper** | 13 | Mapeamento Domain → DTO |
| **RabbitMQ.Client** | 6.8 | Mensageria / Filas |
| **Hangfire + PostgreSql** | 1.8 | Background jobs / Conciliação agendada |
| **JWT Bearer Authentication** | 9 | Autenticação da API |
| **Swagger / Swashbuckle** | 6.5 | Documentação OpenAPI interativa |
| **WireMock** | latest | Mock Server do Itaú (ambiente local) |
| **Docker Compose** | — | Orquestração do ambiente local completo |

---

## Funcionalidades Implementadas

### Credenciais (ClientCredentials)
- Cadastro de credenciais bancárias (ClientId, ClientSecret, certificado mTLS)
- Consulta segura de credencial por ID (campos sensíveis não são expostos em texto puro)
- Revogação permanente de credencial comprometida

### Contas (Accounts)
- Cadastro de conta bancária com vínculo obrigatório a uma credencial
- Consulta de conta por ID
- Atualização de dados bancários (banco, agência, número de conta)
- Ativação/desativação de conta (soft delete — conta inativa não pode emitir Pix)

### Chaves Pix (PixKeys)
- Registro de chave Pix vinculada a uma conta
- Consulta de chave por valor (retorna informações do banco via Adapter)
- Remoção de chave Pix

### Faturas (Invoices)
- Criação de fatura com valor, data de vencimento e referência externa
- A fatura é o documento financeiro pré-requisito para qualquer cobrança Pix

### Cobranças Pix (PixCharges)
- Emissão de cobrança Pix (COB — imediata) com geração de TxId e QR Code EMV
- Consulta de status por TxId (`Active` / `Paid` / `Expired`)
- Reconciliação automática via background job (PollingJob — fallback para webhook perdido)

### Webhooks
- Recepção de notificações de pagamento do banco parceiro
- Processamento assíncrono via fila RabbitMQ (retorna `202 Accepted` instantaneamente)
- Worker em background (`WebhookConsumerWorker`) que consome a fila, parseia o payload e atualiza o status da cobrança para `Paid`

### Infraestrutura
- Auditoria completa de operações (`AuditLog`)
- Logging estruturado com Correlation ID por requisição
- Idempotência por cabeçalho `Idempotency-Key` (cache distribuído, TTL de 24h)
- Tratamento global de exceções no padrão RFC 7807 (ProblemDetails)
- Inicialização automática do banco de dados (`EnsureCreated` no startup)

---

## Referência Completa da API

### Credenciais de Clientes — `/api/v1/client-credentials`

#### `POST /api/v1/client-credentials` — Criar credencial

Cadastra as credenciais bancárias e o certificado digital do lojista. **Este é o primeiro passo obrigatório** antes de criar qualquer conta.

```json
{
  "accountId": "00000000-0000-0000-0000-000000000000",
  "secretValue": "client_id_ou_secret_fornecido_pelo_banco",
  "certificate": "string_base64_do_certificado_mtls",
  "certificatePassword": "senha_do_certificado"
}
```

> **Atenção:** O campo `accountId` aqui é um UUID livre que você gera (use [uuidgenerator.net](https://www.uuidgenerator.net/version4)). Guarde o `id` retornado no `201 Created` — ele se tornará o `secretId` da conta.

| Campo | Tipo | Descrição |
|---|---|---|
| `accountId` | `uuid` | UUID gerado externamente para identificar o vínculo |
| `secretValue` | `string` | ClientId/Secret fornecido pelo banco |
| `certificate` | `string` | Certificado mTLS em Base64 |
| `certificatePassword` | `string` | Senha do certificado |

**Resposta:** `201 Created` → `{ "id": "uuid-da-credencial" }`

---

#### `GET /api/v1/client-credentials/{id}` — Consultar credencial

Retorna metadados da credencial sem expor campos sensíveis.

**Resposta:** `200 OK` → `ClientCredentialDto` | `404 Not Found`

---

#### `POST /api/v1/client-credentials/{id}/revoke` — Revogar credencial

Inativação lógica imediata. Uma credencial revogada **não pode ser reativada**.

**Resposta:** `204 No Content` | `404 Not Found`

---

### Contas — `/api/v1/accounts`

#### `POST /api/v1/accounts` — Criar conta

Vincula a conta bancária física do lojista às credenciais criadas anteriormente.

```json
{
  "secretId": "<ID_RETORNADO_NO_PASSO_1>",
  "document": "12345678909",
  "bankId": "341",
  "accountNumber": "12345",
  "agency": "0001"
}
```

> **Importante:** Use `bankId: "341"` (código COMPE do Itaú) ao usar o Mock Server local.

| Campo | Tipo | Descrição |
|---|---|---|
| `secretId` | `uuid` | ID da credencial criada anteriormente |
| `document` | `string` | CPF (11 dígitos) ou CNPJ (14 dígitos), apenas números |
| `bankId` | `string` | Código COMPE do banco (ex: `"341"` = Itaú) |
| `accountNumber` | `string` | Número da conta bancária |
| `agency` | `string` | Número da agência |

**Resposta:** `201 Created` → `{ "id": "uuid-da-conta" }` | `400 Bad Request`

---

#### `GET /api/v1/accounts/{id}` — Consultar conta

**Parâmetro:** `id` (uuid, path)

**Resposta:** `200 OK` → `AccountDto` | `404 Not Found`

---

#### `PUT /api/v1/accounts/{id}` — Atualizar conta

```json
{
  "id": "<MESMO_ID_DA_URL>",
  "bankId": "341",
  "accountNumber": "54321",
  "agency": "0002"
}
```

> O `id` no body deve ser idêntico ao `id` na URL. Divergências retornam `400`.

**Resposta:** `204 No Content` | `400 Bad Request` | `404 Not Found`

---

#### `PATCH /api/v1/accounts/{id}/status` — Alterar status da conta

Ativa ou desativa uma conta (soft delete). Contas inativas não podem emitir cobranças.

```json
{
  "id": "<MESMO_ID_DA_URL>",
  "isActive": false
}
```

**Resposta:** `204 No Content` | `400 Bad Request` | `404 Not Found`

---

### Chaves Pix — `/api/v1/pix-keys`

#### `POST /api/v1/pix-keys` — Registrar chave Pix

```json
{
  "keyValue": "12345678909",
  "accountId": "<ACCOUNT_ID>"
}
```

**Resposta:** `201 Created` → `{ "id": "uuid-da-chave" }` | `400 Bad Request` | `409 Conflict`

---

#### `GET /api/v1/pix-keys/{keyValue}` — Consultar chave Pix

Retorna a qual conta do Hub a chave pertence, consultando também o banco parceiro via Adapter.

**Parâmetro:** `keyValue` (string, path) — o valor da chave (ex: CPF)

**Resposta:** `200 OK` → `PixKeyInfoDto` | `404 Not Found`

---

#### `DELETE /api/v1/pix-keys/{id}` — Remover chave Pix

**Parâmetro:** `id` (uuid, path)

**Resposta:** `204 No Content` | `404 Not Found`

---

### Faturas — `/api/v1/invoices`

#### `POST /api/v1/invoices` — Criar fatura

Gera o documento financeiro de cobrança. É o pré-requisito para emitir uma cobrança Pix.

```json
{
  "accountId": "<ACCOUNT_ID>",
  "amount": 150.50,
  "dueDate": "2026-06-30T23:59:59.000Z",
  "externalReference": "PEDIDO-1001"
}
```

| Campo | Tipo | Descrição |
|---|---|---|
| `accountId` | `uuid` | Conta que emitirá a cobrança |
| `amount` | `decimal` | Valor em BRL (ex: `150.50`) |
| `dueDate` | `datetime` | Data de vencimento ISO 8601 |
| `externalReference` | `string` | Referência do sistema do cliente (ex: número do pedido) |

**Resposta:** `201 Created` → `{ "id": "uuid-da-fatura" }` | `400 Bad Request`

---

### Cobranças Pix — `/api/v1/pix-charges`

#### `POST /api/v1/pix-charges` — Emitir cobrança Pix

Integra com a API do banco (via Adapter/Mock) para gerar o TxId e o QR Code EMV.

```json
{
  "invoiceId": "<INVOICE_ID>"
}
```

**Resposta:** `201 Created`

```json
{
  "txId": "HUB20260611110339abc123",
  "emv": "00020101021226860014br.gov.bcb.pix...",
  "qrCodeBase64": "data:image/png;base64,..."
}
```

| `400 Bad Request` | `404 Not Found` (invoice não existe)

---

#### `GET /api/v1/pix-charges/{txId}/status` — Consultar status

Endpoint de leitura otimizada para polling do frontend. Retorna o estado atual da cobrança.

**Parâmetro:** `txId` (string, path)

**Resposta:** `200 OK` → `"Active"` | `"Paid"` | `"Expired"` | `404 Not Found`

---

### Webhooks — `/api/v1/webhooks`

#### `POST /api/v1/webhooks/pix` — Receber notificação de pagamento

Porta de entrada de alta disponibilidade para notificações do banco parceiro. Não requer JWT convencional (utiliza mTLS ou HMAC em produção).

```json
{
  "pix": [
    {
      "endToEndId": "E3410000020260611110339669000000",
      "txid": "<TX_ID_DA_COBRANÇA>",
      "valor": "150.50",
      "horario": "2026-06-11T11:03:39.669Z"
    }
  ]
}
```

**Resposta:** `202 Accepted` (imediato — o processamento ocorre de forma assíncrona no Worker)

> O Hub **não processa o pagamento na thread da requisição**. Ele captura o JSON, enfileira no RabbitMQ e libera a conexão do banco. O `WebhookConsumerWorker` faz o processamento em background.

---

## Fluxo de Negócio Completo

O diagrama abaixo representa o ciclo de vida completo de uma cobrança Pix no Hub, do cadastro inicial até a confirmação de pagamento.

```
┌─────────────────────────────────────────────────────────────────┐
│                    FLUXO COMPLETO DE OPERAÇÃO                   │
└─────────────────────────────────────────────────────────────────┘

  [1] Criar ClientSecret
       POST /api/v1/client-credentials
       → Retorna: secretId
            │
            ▼
  [2] Criar Account
       POST /api/v1/accounts  {secretId}
       → Retorna: accountId
            │
            ▼
  [3] Registrar Chave Pix (opcional, mas necessário para receber)
       POST /api/v1/pix-keys  {accountId}
            │
            ▼
  [4] Criar Invoice (fatura)
       POST /api/v1/invoices  {accountId, amount, dueDate}
       → Retorna: invoiceId
            │
            ▼
  [5] Emitir Cobrança Pix
       POST /api/v1/pix-charges  {invoiceId}
       → Hub gera TxId, chama Adapter Itaú, obtém QR Code
       → Retorna: { txId, emv, qrCodeBase64 }
            │
            ▼
  [6] Cliente paga o QR Code
       (ação do usuário fora do Hub)
            │
            ├──── Via WEBHOOK (caminho principal) ────────────────┐
            │     POST /api/v1/webhooks/pix  {txid}               │
            │     → 202 Accepted (enfileira no RabbitMQ)          │
            │     → WebhookConsumerWorker processa em background   │
            │     → PixCharge.Status = Paid                       │
            │                                                     │
            └──── Via POLLING (fallback automático) ──────────────┘
                  PollingJob verifica cobranças pendentes
                  → Consulta status no banco parceiro
                  → Atualiza PixCharge.Status se necessário
            │
            ▼
  [7] Verificar Status
       GET /api/v1/pix-charges/{txId}/status
       → Retorna: "Paid"
```

### Regra de Ouro do Banco de Dados

**Integridade referencial exige ordem.** Cada passo depende do ID retornado no anterior:

```
secretId (do passo 1) → obrigatório para criar Account
accountId (do passo 2) → obrigatório para criar PixKey e Invoice
invoiceId (do passo 4) → obrigatório para criar PixCharge
txId (do passo 5) → usado para verificar status e no webhook
```

---

## Integração Bancária

### Abstração via Adapter Pattern

Toda comunicação com bancos parceiros passa pela interface `IBankPixAdapter`. Isso isola completamente as particularidades de autenticação, endpoints e formatos de payload de cada banco.

```csharp
public interface IBankPixAdapter
{
    Task<ChargeResponseDto> GeneratePixAsync(ChargeRequestDto request);
    Task<PixKeyInfoDto> GetPixKeyAsync(string keyValue);
    Task<string> GetChargeStatusAsync(string txId);
}
```

A seleção do Adapter correto é feita pela `BankAdapterFactory` com base no `BankId` da conta:

```csharp
return bankId switch
{
    "341" => serviceProvider.GetRequiredService<ItauPixAdapter>(),
    // "001" => BancoDoBrasilPixAdapter (planejado)
    // "237" => BradescoPixAdapter (planejado)
    _ => throw new NotSupportedException($"Banco '{bankId}' não suportado.")
};
```

### Status de Implementação por Banco

| Banco | Código COMPE | Status | Observação |
|---|---|---|---|
| **Itaú** | 341 | ✅ Implementado | Mock via WireMock disponível |
| Banco do Brasil | 001 | 🔲 Planejado | |
| Bradesco | 237 | 🔲 Planejado | |
| Santander | 033 | 🔲 Planejado | |

### Adicionar um Novo Banco (Para Desenvolvedores)

Para adicionar suporte a um novo banco, são necessários apenas 3 passos:

1. Criar `NovoBancoPixAdapter.cs` em `Infrastructure/BankAdapters/NovoBanco/` implementando `IBankPixAdapter`
2. Criar `NovoBancoOptions.cs` com as configurações do banco
3. Registrar na `BankAdapterFactory` adicionando a linha `"001" => serviceProvider.GetRequiredService<NovoBancoPixAdapter>()` e registrar o `HttpClient` no `DependencyInjection.cs`

As camadas de Domain e Application **não precisam de nenhuma alteração**.

---

## Segurança

### Autenticação JWT

A API utiliza JWT Bearer Authentication configurada no Swagger (botão "Authorize"). Todas as requisições às rotas protegidas devem incluir o cabeçalho:

```
Authorization: Bearer {seu_token_jwt}
```

A chave de assinatura e o Issuer são configurados em `appsettings.json`:

```json
{
  "Jwt": {
    "Key": "SUA_CHAVE_SUPER_SECRETA_LONGA_O_SUFICIENTE",
    "Issuer": "HubBancarioAPI",
    "Audience": "HubBancarioClients"
  }
}
```

### Credenciais Bancárias (ClientSecret)

O `Certificate` (certificado mTLS) é armazenado como `text` no PostgreSQL, suportando strings imensas como chaves PEM ou certificados Base64. Campos sensíveis não são retornados em consultas públicas.

### Idempotência

O `IdempotencyFilter` protege todas as operações de mutação (`POST`, `PUT`, `PATCH`). O ERP cliente **deve** enviar o cabeçalho:

```
Idempotency-Key: {uuid-único-por-operação}
```

Requisições com o mesmo `Idempotency-Key` recebidas dentro de 24 horas retornam `409 Conflict` sem executar a operação novamente. Isso previne cobranças duplicadas em caso de timeout ou retry do cliente.

### Webhook — Segurança em Produção

O endpoint de webhook aceita qualquer `JsonElement` no ambiente de desenvolvimento. Em produção, deve-se implementar validação de assinatura HMAC-SHA256 ou mTLS conforme exigência do banco parceiro, lendo o header `X-Itau-Signature` antes de enfileirar a mensagem.

### Proteção Global contra Erros

O `ExceptionMiddleware` captura qualquer exceção não tratada e retorna uma resposta padronizada RFC 7807 sem vazar stack traces para o cliente:

```json
{
  "type": "https://tools.ietf.org/html/rfc7807",
  "title": "Erro interno",
  "status": 500,
  "detail": "Ocorreu um erro ao processar sua requisição."
}
```

---

## Ambiente Docker — Executando o Projeto

### Pré-requisitos

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado e em execução
- [Git](https://git-scm.com/) para clonar o repositório

### Serviços do Ambiente Local

| Serviço | Container | Porta Externa | Finalidade |
|---|---|---|---|
| **API** | `bankinghub-api` | `5000` | A aplicação .NET |
| **PostgreSQL** | `bankinghub-postgres` | `5432` | Banco de dados |
| **RabbitMQ** | `bankinghub-rabbitmq` | `5672` / `15672` | Mensageria / Painel de gestão |
| **WireMock (Mock Itaú)** | `mock-bank-itau` | `8080` | Simulador do banco Itaú |

### Passo a Passo para Executar

**1. Clone o repositório**

```bash
git clone https://github.com/nicolascleik/HubBancario-UNIT.git
cd HubBancario-UNIT
```

**2. ⚠️ Navegue para a pasta `docker` — este passo é obrigatório**

```bash
cd docker
```

> O `docker-compose.yml` usa caminhos relativos para o `Dockerfile` e para os mapeamentos do WireMock. Executar o comando de qualquer outra pasta causará erros de build.

**3. Suba todos os serviços**

```bash
docker-compose up -d --build
```

O flag `--build` garante que a imagem da API seja (re)construída com o código mais recente. O flag `-d` roda tudo em background.

**4. Aguarde a inicialização**

Na primeira execução, o Docker irá baixar as imagens do PostgreSQL, RabbitMQ e WireMock. Isso pode levar alguns minutos dependendo da sua conexão. A API só sobe após o PostgreSQL estar saudável (healthcheck configurado no compose).

**5. Acesse o Swagger**

Abra no navegador:

```
http://localhost:5000/swagger
```

A interface do Swagger listará todos os endpoints disponíveis com a documentação completa de payloads e respostas.

### Verificando os Serviços

```bash
# Ver status de todos os containers
docker-compose ps

# Ver logs da API em tempo real
docker-compose logs -f api

# Ver logs do Worker de webhook
docker-compose logs -f api | grep "Worker"
```

### Painel do RabbitMQ

Acesse `http://localhost:15672` com as credenciais:
- **Usuário:** `guest`
- **Senha:** `guest`

Aqui você pode monitorar a fila `webhook_events_queue` e ver as mensagens sendo consumidas em tempo real pelo `WebhookConsumerWorker`.

### Parar o Ambiente

```bash
# Para os containers mas mantém os dados
docker-compose stop

# Remove os containers (dados do PostgreSQL são preservados no volume)
docker-compose down

# Remove tudo, incluindo o volume do banco de dados
docker-compose down -v
```

### Variáveis de Ambiente Configuradas Automaticamente

O `docker-compose.yml` injeta todas as variáveis necessárias na API automaticamente:

| Variável | Valor no Docker | Descrição |
|---|---|---|
| `ConnectionStrings__DefaultConnection` | `Host=postgres;...` | Aponta para o PostgreSQL do compose |
| `BankAdapters__Itau__BaseUrl` | `http://mock-bank:8080` | Aponta para o WireMock |
| `BankAdapters__Itau__ClientId` | `mock-client-id` | Credencial fake do mock |
| `RabbitMq__Host` | `rabbitmq` | Nome do serviço no compose |

---

## Guia de Testes End-to-End (E2E)

Este guia simula o ciclo de vida real de uma cobrança Pix, do cadastro inicial até a confirmação de pagamento. Execute os passos em ordem no Swagger (`http://localhost:5000/swagger`).

> **Dica:** Gere UUIDs em [uuidgenerator.net](https://www.uuidgenerator.net/version4). Copie sempre o `id` retornado em cada resposta `201 Created` e cole no payload do passo seguinte.

---

### Passo 1 — Criar Credenciais

**Endpoint:** `POST /api/v1/client-credentials`

```json
{
  "accountId": "bd061ca5-46a9-4c49-9082-2b0d7ceb6ee3",
  "secretValue": "mock-client-secret-do-itau",
  "certificate": "certificado_base64_aqui",
  "certificatePassword": "senha123"
}
```

📋 **Copie o `id` retornado → este é o seu `SecretId`.**

---

### Passo 2 — Criar Conta Bancária

**Endpoint:** `POST /api/v1/accounts`

```json
{
  "secretId": "<COLE_O_SECRET_ID_DO_PASSO_1>",
  "document": "12345678909",
  "bankId": "341",
  "accountNumber": "12345",
  "agency": "0001"
}
```

> Use `bankId: "341"` (Itaú) para funcionar com o Mock Server.

📋 **Copie o `id` retornado → este é o seu `AccountId`.**

---

### Passo 3 — Registrar Chave Pix

**Endpoint:** `POST /api/v1/pix-keys`

```json
{
  "keyValue": "12345678909",
  "accountId": "<COLE_O_ACCOUNT_ID_DO_PASSO_2>"
}
```

---

### Passo 4 — Criar Fatura

**Endpoint:** `POST /api/v1/invoices`

```json
{
  "accountId": "<COLE_O_ACCOUNT_ID_DO_PASSO_2>",
  "amount": 150.50,
  "dueDate": "2026-12-31T23:59:59.000Z",
  "externalReference": "PEDIDO-1001"
}
```

📋 **Copie o `id` retornado → este é o seu `InvoiceId`.**

---

### Passo 5 — Emitir Cobrança Pix

**Endpoint:** `POST /api/v1/pix-charges`

```json
{
  "invoiceId": "<COLE_O_INVOICE_ID_DO_PASSO_4>"
}
```

O Hub irá:
1. Buscar a fatura e a conta no banco
2. Gerar um `TxId` único no padrão BACEN
3. Chamar o Mock Itaú (WireMock) para criar a cobrança
4. Buscar o QR Code EMV no WireMock
5. Salvar a `PixCharge` com status `Active`
6. Retornar o `txId` e o `emv`

📋 **Copie o `txId` retornado → você usará nos próximos passos.**

---

### Passo 6 — Simular Pagamento via Webhook

**Endpoint:** `POST /api/v1/webhooks/pix`

```json
{
  "pix": [
    {
      "endToEndId": "E3410000020260611110339669000000",
      "txid": "<COLE_O_TX_ID_DO_PASSO_5>",
      "valor": "150.50",
      "horario": "2026-06-11T11:03:39.669Z"
    }
  ]
}
```

A API retornará `202 Accepted` imediatamente. Em background, o `WebhookConsumerWorker`:
1. Consome a mensagem da fila RabbitMQ
2. Parseia o `txId` do payload
3. Envia o `ProcessWebhookCommand` ao MediatR
4. Atualiza o status da cobrança para `Paid`

---

### Passo 7 — Validar Confirmação de Pagamento

**Endpoint:** `GET /api/v1/pix-charges/{txId}/status`

Substitua `{txId}` pelo valor do Passo 5.

**Resultado esperado:**
```json
"Paid"
```

> Se o status ainda for `"Active"`, aguarde 1-2 segundos para o Worker processar a fila e tente novamente.

---

## Mock Itaú (WireMock)

O ambiente Docker inclui um servidor WireMock configurado para simular as APIs do Itaú. Todos os cenários são configurados pelos arquivos JSON em `docker/wiremock/mappings/`.

### Cenários Disponíveis

| Arquivo | Método | URL | Cenário Simulado |
|---|---|---|---|
| `01-itau-token.json` | `POST` | `/oauth2/token` | Geração de token OAuth2 |
| `02-create-cobv-success.json` | `PUT` | `/cobv/{txid}` | Criação de COBV com sucesso |
| `03-create-cob-success.json` | `PUT` | `/cob/{txid}` | Criação de COB com sucesso |
| `04-qrcode-cobv.json` | `GET` | `/cobv/{txid}/qrcode` | Geração de QR Code EMV |
| `05-status-active-first.json` | `GET` | `/(cob\|cobv)/{txid}` | Status ATIVA (1ª consulta) |
| `06-status-paid-second.json` | `GET` | `/(cob\|cobv)/{txid}` | Status CONCLUIDA (2ª consulta) |
| `07-status-expired.json` | `GET` | `/EXPIRADA{txid}` | Status EXPIRADA |
| `08-bank-error.json` | `GET` | `/ERRO{txid}` | Erro 500 do banco |

### Cenário de Estado (Stateful Mock)

Os mapeamentos `05` e `06` usam o mecanismo de **Scenarios** do WireMock para simular a progressão de status:

- **1ª consulta de status** → Retorna `"ATIVA"` e avança o cenário
- **2ª consulta de status** → Retorna `"CONCLUIDA"` com dados do pagamento

Isso replica o comportamento real onde o banco leva alguns instantes para confirmar o pagamento.

### Exemplo de Uso Direto do Mock

```bash
# Criar uma cobrança com vencimento diretamente no mock
curl -X PUT http://localhost:8080/cobv/MEUPIX123 \
  -H "Content-Type: application/json" \
  -d '{"valor": {"original": "150.00"}}'

# Consultar status
curl http://localhost:8080/cobv/MEUPIX123

# Forçar erro do banco
curl http://localhost:8080/cobv/ERRO_FORÇADO
```

---

## Testes Automatizados

O projeto documenta uma estratégia de testes baseada na **Pirâmide Inteligente**, reconhecendo que testes unitários isolados podem "mentir" ao mockar infraestrutura real.

### Stack de Testes

| Biblioteca | Finalidade |
|---|---|
| **xUnit** | Framework de testes |
| **Moq** | Simulações para testes de Application |
| **TestContainers** | PostgreSQL e RabbitMQ reais via Docker nos testes |
| **WireMock.Net** | Mock HTTP para simular o Itaú nos testes de integração |
| **Microsoft.AspNetCore.Mvc.Testing** | API em memória para testes E2E |
| **FluentAssertions** | Asserções legíveis |

### Camadas de Teste

#### 1. Testes de Domínio — Unitários Puros

**Localização:** `Domain` layer  
**Ferramenta:** xUnit (sem Moq)  
**Foco:** Regras de negócio puras — o Domínio não possui dependências de infraestrutura, portanto os testes são 100% confiáveis.

Exemplos do que testar:
- Criar um `Money` com valor negativo deve lançar `DomainException`
- Revogar uma credencial já revogada deve lançar exceção
- `Document.Create` com CPF inválido (dígitos verificadores errados) deve falhar
- `Account.ChangeStatus` para o mesmo status atual deve lançar exceção

#### 2. Testes de Application — Comportamentais

**Localização:** `Application` layer  
**Ferramenta:** xUnit + Moq  
**Foco:** O fluxo de decisão dos Handlers, não se o dado foi salvo (isso seria testar o Moq).

Exemplos do que testar:
- `CreateAccountHandler` chamou `_accountRepository.AddAsync()` exatamente uma vez?
- `ValidationBehavior` barrou o comando quando o `SecretId` está vazio?
- `ProcessWebhookHandler` acionou o `NotificationService` após atualizar o status?

#### 3. Testes de Infraestrutura — Integração Real

**Localização:** `Infrastructure` layer  
**Ferramenta:** TestContainers  
**Foco:** Verificar que o SQL gerado pelo EF Core realmente persiste os dados corretamente no PostgreSQL.

O TestContainers sobe um PostgreSQL real via Docker, cria as tabelas, executa o teste e destrói o container — sem poluir o ambiente de desenvolvimento.

#### 4. Testes de API — E2E Completo

**Localização:** API layer  
**Ferramenta:** `WebApplicationFactory` + TestContainers  
**Foco:** A jornada completa ponta a ponta — simula um ERP externo fazendo chamadas HTTP à API.

```csharp
// Exemplo de estrutura do teste E2E
public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("bankinghub_test")
        .WithUsername("admin")
        .WithPassword("admin")
        .Build();

    public async Task InitializeAsync()
        => await _dbContainer.StartAsync();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Substitui o banco de produção pelo TestContainers
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<BankingDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<BankingDbContext>(options =>
                options.UseNpgsql(_dbContainer.GetConnectionString()));
        });
    }
}
```

### Pontos de Integração (Para a Equipe de QA)

Os testes de integração cobrem 6 pontos de costura do sistema:

| Ponto | Camadas | O que valida |
|---|---|---|
| **A** | Application ↔ Infrastructure | Mapeamentos EF Core, chaves estrangeiras, transações |
| **B** | Infrastructure ↔ Banco Itaú | Formatação do payload, cabeçalhos HTTP, tradução da resposta |
| **C** | API ↔ Application ↔ Infra | Jornada E2E completa ("Caminho Feliz") |
| **D** | RabbitMQ ↔ Worker | Processamento assíncrono, atualização de status pós-fila |
| **E** | Hub → ERP do cliente | Entrega correta do webhook de saída (notificação B2B) |
| **F** | Middlewares ↔ API | JWT, idempotência, tratamento de exceções (RFC 7807) |

> **Nota:** Os testes de integração (Pontos A–F) foram documentados como casos a serem implementados pelos próximos desenvolvedores. Se um teste de integração falhar, ele indica um bug genuíno no código — não o ignore. Rastreie a causa raiz e corrija o código-fonte antes que o problema chegue em produção.

---

## Evolução do Projeto — Do Modelo Antigo ao Novo

Este projeto passou por uma evolução significativa de escopo e arquitetura durante seu desenvolvimento. Esta seção documenta as principais mudanças para contextualizar os desenvolvedores futuros.

### O Que Mudou e Por Quê

**1. Centralização de Responsabilidades**

O Hub deixou de ser um simples roteador e passou a ser o dono de toda a regra de negócio bancária. Isso incluiu a criação do banco de dados próprio, do modelo de Account/Secret e do ciclo completo de Invoice → PixCharge.

**2. Remoção do NGINX**

O NGINX foi removido do ambiente Docker para simplificar o desenvolvimento e o entendimento do fluxo. A comunicação segura via mTLS passa a ser responsabilidade da configuração da aplicação em produção, não de um proxy reverso no ambiente local.

**3. Padronização de Nomenclatura**

A entidade `Client` foi renomeada para `Account` em todo o sistema para alinhar o vocabulário ao domínio bancário brasileiro (Open Finance). Isso impactou controllers, commands, handlers, repositories e configurações do EF Core.

**4. Implementação do Webhook Consumer**

O `WebhookConsumerWorker` foi criado como `BackgroundService` para consumir a fila `webhook_events_queue` do RabbitMQ. Ele implementa o padrão de `BasicAck`/`BasicNack` para garantir que mensagens não sejam perdidas em caso de falha.

**5. Correção do WebhooksController**

O controller foi corrigido para receber `[FromBody] JsonElement payload` em vez de ler o `Request.Body` como stream bruto, resolvendo problemas de compatibilidade com o pipeline do ASP.NET Core.

**6. Criação do DomainToDtoMappingProfile**

O perfil de mapeamento AutoMapper foi centralizado em `DomainToDtoMappingProfile.cs`, com mapeamentos explícitos para Value Objects (ex: `src.Document.Value` → `dest.Document`) e exclusões de campos não mapeáveis.

**7. Gestão de Estados de Pagamento**

Foi implementada a lógica de gerenciamento de estados da cobrança Pix (`Active` → `Paid` / `Expired`), garantindo transições de estado coerentes e auditáveis.

### O Que Permanece Planejado

- Implementação dos Adapters para Banco do Brasil, Bradesco e Santander
- Completar os testes de integração nos 6 pontos documentados
- Implementação da validação HMAC-SHA256 no WebhooksController para produção
- Evolução do ItauPixAdapter com suporte completo a mTLS em produção

---

## Roadmap e Desenvolvimento Futuro

### Em Desenvolvimento

- Evolução do `ItauPixAdapter` (mTLS em produção, renovação de token, retry policy)
- Cobertura de testes de integração nos Pontos A–F
- Melhorias na conciliação automática (ReconciliationJob)

### Planejado — Novos Bancos

| Banco | Código COMPE | Prioridade |
|---|---|---|
| Banco do Brasil | 001 | Alta |
| Bradesco | 237 | Média |
| Santander | 033 | Média |

### Planejado — Novas Funcionalidades

- **Split Pix** — divisão do pagamento entre múltiplos recebedores
- **Pix Automático** — débito programado
- **Chargeback** — estorno de pagamentos
- **Dashboard de Auditoria** — visualização do AuditLog
- **Notificações B2B** — webhook de saída para o ERP do cliente após confirmação de pagamento

---

## Princípios de Negócio

### Webhook não é Fonte de Verdade

Os webhooks recebidos dos bancos funcionam apenas como **gatilhos** para iniciar o processo de validação. A confirmação definitiva de um pagamento ocorre exclusivamente através de **consulta ativa ao banco emissor** (polling via Adapter). Isso garante que pagamentos não sejam creditados com base em notificações forjadas ou corrompidas.

### TxId como Chave de Conciliação Universal

Todo pagamento Pix é identificado pelo `TxId` gerado pelo Hub. Este identificador é utilizado tanto na comunicação com o banco quanto internamente para rastrear o ciclo de vida completo da cobrança, desde a criação até a confirmação do pagamento.

### Idempotência por Design

O sistema foi projetado para suportar eventos duplicados, atrasados ou fora de ordem sem gerar efeitos colaterais indesejados. O `IdempotencyFilter` na API e o tratamento de `BasicNack` no Worker garantem que a mesma operação nunca seja executada duas vezes.

### Auditoria Completa e Imutável

Toda operação sensível — criação de conta, emissão de cobrança, recepção de webhook, mudança de status — é registrada no `AuditLog` de forma imutável. Isso garante rastreabilidade completa para fins de compliance e investigação de incidentes.

### Resiliência por Fallback

O sistema opera com dois mecanismos de conciliação complementares:

1. **Primário (reativo):** O banco envia um webhook → Hub enfileira → Worker processa → Status atualizado
2. **Fallback (proativo):** Se o webhook falhar, o `PollingJob` (Hangfire) consulta ativamente o banco a cada 10 minutos para cobranças com status `Active` há mais tempo que o esperado

---

## Informações Institucionais e Equipe

Este projeto foi proposto e desenvolvido através de uma parceria entre o **Porto Digital** e a **Receba Digital**, sendo executado no escopo acadêmico da **Universidade Tiradentes (UNIT)**.

- **Módulo/Disciplina:** Residência II GP0029VNO03C - 2026.1
- **Mentor:** Eduardo Wyne
- **Squad:** 09

### Integrantes

- DANIEL LIMA PEREIRA
- DAVI TODT DIAS MOURA
- FERNANDO PIRICHOWSKI AGUIAR
- GUILHERME DOS SANTOS GUIMARÃES
- LUÍS FELIPE COSTA RIBEIRO SILVA
- MATEUS BAGUES ARAUJO OLIVEIRA SANTOS
- NICOLAS CLEIK DE ANDRADE (Líder)

---

## Status do Projeto

**Entrega Final**

O escopo principal solicitado pela Receba Digital foi integralmente finalizado. O projeto encontra-se totalmente funcional para o fluxo de negócio principal (Passos 1–7 do guia E2E), operando com a integração via Mock do Itaú.

**Entregas Adicionais (Extra):** 
Embora não fosse um requisito obrigatório para a aprovação final do projeto, foi desenvolvida e entregue toda a documentação e estratégia para os **Testes de Integração**. Esse incremento foi planejado e estruturado para fornecer à equipe da Receba Digital uma base sólida de segurança e escalabilidade, facilitando o trabalho da equipe futura que dará continuidade ao desenvolvimento.

**Repositório:** https://github.com/nicolascleik/HubBancario-UNIT

---

*Desenvolvido para a Receba Digital — Hub de Integração Bancária Open Finance*