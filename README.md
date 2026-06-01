# BankingHub - Hub de Integração Bancária Open Finance

## Sobre o Projeto

O **BankingHub** é uma plataforma de integração bancária desenvolvida para centralizar operações Pix dentro do ecossistema **Open Banking/Open Finance brasileiro**.

O projeto foi concebido seguindo o princípio **API First**, permitindo que qualquer aplicação consuma serviços bancários através de uma interface única, padronizada e desacoplada das implementações específicas de cada instituição financeira. :contentReference[oaicite:1]{index=1}

A arquitetura foi projetada para minimizar lock-in tecnológico, garantir auditabilidade completa das operações e permitir a inclusão de novos bancos sem impacto nas regras centrais de negócio. Atualmente, o Itaú é o primeiro provedor implementado através do padrão Adapter. :contentReference[oaicite:2]{index=2}

---

## Objetivos do Projeto

O BankingHub tem como principais objetivos:

- Emitir cobranças Pix (Cob e CobV)
- Validar pagamentos através de consulta ativa ao banco
- Permitir integração com múltiplas instituições financeiras
- Garantir idempotência das operações
- Fornecer mecanismos de conciliação automática
- Centralizar integrações bancárias em uma API única
- Reduzir acoplamento entre aplicações consumidoras e bancos
- Seguir os padrões definidos pelo Open Finance brasileiro

---

## Arquitetura

O projeto foi projetado seguindo os seguintes princípios arquiteturais:

- API First
- Clean Architecture
- Domain-Driven Design (DDD)
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Unit of Work
- Adapter Pattern
- Factory Pattern
- Strategy Pattern

Esses padrões garantem separação de responsabilidades, alta testabilidade, facilidade de manutenção e expansão futura para novos provedores bancários. :contentReference[oaicite:3]{index=3}

---

## Estrutura do Projeto

```text
HubBancario-UNIT
│
├── HubBancario
│   ├── API
│   ├── Application
│   ├── Domain
│   ├── Infrastructure
│   └── Properties
│
├── docker
│   ├── wiremock
│   │   └── mappings
│   ├── Dockerfile
│   └── docker-compose.yml
│
├── HubBancario.sln
└── README.md
```

### Responsabilidade das Camadas

| Camada | Responsabilidade |
|----------|----------------|
| API | Exposição dos endpoints HTTP, autenticação e middlewares |
| Application | Casos de uso, Commands, Queries e validações |
| Domain | Regras de negócio, entidades, agregados e eventos |
| Infrastructure | Persistência, mensageria, jobs e integrações externas |

---

## Tecnologias Utilizadas

| Tecnologia | Finalidade |
|------------|------------|
| .NET 10 | Plataforma principal |
| ASP.NET Core | API REST |
| Entity Framework Core | Persistência |
| PostgreSQL | Banco de dados |
| MediatR | Implementação de CQRS |
| FluentValidation | Validação de comandos |
| AutoMapper | Mapeamento de objetos |
| RabbitMQ | Mensageria |
| Hangfire | Processamento em background |
| JWT Bearer Authentication | Autenticação |
| Swagger / OpenAPI | Documentação da API |
| WireMock | Simulação de integrações bancárias |
| Docker Compose | Ambiente local |

---

## Funcionalidades Implementadas

### Contas

- Cadastro de contas
- Atualização de contas
- Remoção de contas

### Cobranças Pix

- Criação de cobrança Pix imediata (Cob)
- Criação de cobrança Pix com vencimento (CobV)
- Consulta de status por TxId
- Reconciliação manual de cobranças
- Geração de QR Code EMV

### Chaves Pix

- Cadastro de chave Pix
- Remoção de chave Pix

### Webhooks

- Recebimento de notificações bancárias
- Processamento assíncrono de eventos

### Infraestrutura

- Integração bancária via Adapter Pattern
- RabbitMQ para comunicação assíncrona
- Hangfire para processamento de jobs
- Auditoria e rastreabilidade de operações
- Logging e correlação de requisições

---

## Princípios de Negócio

### Webhook não é Fonte de Verdade

Os webhooks recebidos dos bancos funcionam apenas como gatilhos para iniciar o processo de validação.

A confirmação definitiva de um pagamento ocorre exclusivamente através de consulta ativa ao banco emissor. :contentReference[oaicite:4]{index=4}

### TxId como Chave de Conciliação

Todo pagamento Pix é reconciliado utilizando o **TxId** como identificador universal entre os sistemas. :contentReference[oaicite:5]{index=5}

### Idempotência por Design

O sistema foi projetado para suportar eventos duplicados, atrasados ou fora de ordem sem gerar efeitos colaterais indesejados. :contentReference[oaicite:6]{index=6}

### Auditoria Completa

As integrações bancárias são projetadas para permitir rastreabilidade completa de requests e responses trocados com as instituições financeiras. :contentReference[oaicite:7]{index=7}

---

## Integração Bancária

A integração com bancos é realizada através da abstração:

```csharp
IBankPixAdapter
```

Cada instituição financeira possui sua própria implementação, isolando particularidades de autenticação, endpoints e payloads.

Atualmente:

| Banco | Status |
|---------|---------|
| Itaú | Implementado |
| Banco do Brasil | Planejado |
| Bradesco | Planejado |
| Santander | Planejado |

---

## Módulos da API

Controllers atualmente disponíveis:

```text
AccountsController
ClientSecretsController
InvoicesController
PixChargesController
PixKeysController
WebhooksController
```

### Operações Identificadas

#### Contas

```http
POST
PUT {id}
DELETE {id}
```

#### Cobranças Pix

```http
POST
GET {txId}
POST {txId}/reconcile
```

#### Chaves Pix

```http
POST
DELETE {id}
```

#### Webhooks

```http
POST /pix
```

> Consulte a documentação OpenAPI/Swagger para visualizar os endpoints completos e contratos atualizados.

---

## Ambiente Local

O ambiente Docker disponibiliza:

| Serviço | Porta |
|----------|--------|
| API | 5000 |
| WireMock (Mock Itaú) | 8080 |
| RabbitMQ Management | 15672 |
| PgAdmin | 5050 |

---

## Executando o Projeto

Acesse a pasta Docker:

```bash
cd docker
```

Suba o ambiente:

```bash
docker compose up --build
```

Após a inicialização, consulte a documentação OpenAPI/Swagger disponível na aplicação.

---

## Mock Itaú (WireMock)

O ambiente local utiliza WireMock para simular integrações Pix do Itaú.

### Cenários Disponíveis

- Geração de Token OAuth2
- Criação de Cob
- Criação de CobV
- Geração de QR Code
- Cobrança ativa
- Cobrança concluída
- Cobrança expirada
- Erros bancários simulados

### Exemplo

Criar cobrança:

```bash
curl -X PUT http://localhost:8080/cobv/PIX123456
```

Consultar status:

```bash
curl http://localhost:8080/cobv/PIX123456
```

---

## Middlewares

A API utiliza middlewares customizados para:

- Tratamento global de exceções
- Correlation ID
- Logging de requisições

```csharp
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
```

---

## Roadmap

### Em Desenvolvimento

- Evolução do Adapter Itaú
- Melhorias de conciliação
- Expansão de cobertura de testes

### Planejado

- Banco do Brasil
- Bradesco
- Santander
- Split Pix
- Chargeback
- Pix Automático

---

## Status do Projeto

🚧 Projeto em desenvolvimento.

