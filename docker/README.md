# BankingHub Docker + Mock Itaú

Estrutura para subir um ambiente local com:

- API .NET do HubBancario
- WireMock simulando endpoints Pix do Itaú
- PostgreSQL
- RabbitMQ
- PgAdmin

## Como usar

Coloque esta pasta `docker` na raiz do projeto, no mesmo nível de `HubBancario/`.

```bash
cd docker
docker compose up --build
```

Acessos:

- API: http://localhost:5000
- Mock Itaú: http://localhost:8080
- RabbitMQ: http://localhost:15672
- PgAdmin: http://localhost:5050

## Testes rápidos do mock

Criar CobV:

```bash
curl -X PUT http://localhost:8080/cobv/PIX123456
```

Consultar primeira vez:

```bash
curl http://localhost:8080/cobv/PIX123456
```

Consultar segunda vez:

```bash
curl http://localhost:8080/cobv/PIX123456
```

Na primeira consulta o mock retorna `ATIVA`. Na segunda, retorna `CONCLUIDA` com array `pix`, exatamente no formato esperado pelo `ItauPixAdapter`.

## Ajustes prováveis

Se o nome da DLL da API for diferente, ajuste no Dockerfile:

```dockerfile
ENTRYPOINT ["dotnet", "HubBancario.dll"]
```

Se o caminho do `.csproj` for diferente, ajuste:

```dockerfile
RUN dotnet publish ./HubBancario/HubBancario.csproj
```


