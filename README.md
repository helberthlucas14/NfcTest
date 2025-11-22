# NfcTest — Setup, decisões técnicas e exemplos

## Setup Rápido
- Pré-requisitos: `Docker` e `Docker Compose` instalados.
- Subir todo o ambiente:
  - `docker compose up -d`
- URLs principais:
  - API: `http://localhost:8085`
  - Frontend: `http://localhost:8082`
  - Hangfire Dashboard: `http://localhost:8085/hangfire`
  - Seq (UI): `http://localhost:8081` — ingest `http://localhost:5341`
  - Prometheus: `http://localhost:9090`
  - Grafana: `http://localhost:3000`
  - LocalStack (S3): `http://localhost:4566`

## Serviços (Docker)
- `nfc.api` expõe a API em `8085` (mapa `8085:8080`) e roda em ambiente `Docker` com `ASPNETCORE_URLS=http://+:8080` (docker-compose.yml:122-148).
- `nfc.web` expõe o frontend Angular em `8082` (docker-compose.yml:152-163).
- `nfc_redis_db` expõe Redis em `6379` (docker-compose.yml:27-37).
- `nfcDb` expõe SQL Server em `1433` e roda com `SA_PASSWORD` e `ACCEPT_EULA` (docker-compose.yml:2-13).
- `localstack` inicia `s3` e expõe `4566` (docker-compose.yml:39-57).
- `seq` expõe UI `8081` e ingest `5341` (docker-compose.yml:79-95).
- `prometheus` e `grafana` expõem `9090` e `3000` respectivamente (docker-compose.yml:97-121).

## Backend (API)
- Pipeline e mapeamentos:
  - OpenAPI/Scalar habilitados em `Development` e `Docker` (backend/NfcTest/src/Nfc.Api/Program.cs:36-40).
  - CORS liberado (backend/NfcTest/src/Nfc.Api/Program.cs:25-29, 42).
  - Middleware de `CorrelationId` (backend/NfcTest/src/Nfc.Api/Program.cs:46).
  - Hangfire Dashboard mapeado em `/hangfire` (backend/NfcTest/src/Nfc.Api/Program.cs:50).
  - SignalR Hub para status de exportação em `/hubs/export-status` (backend/NfcTest/src/Nfc.Api/Program.cs:56).
- Hangfire + Redis:
  - Registro de Hangfire com armazenamento Redis e prefixo `hangfire:` (backend/NfcTest/src/Nfc.Infra.HangFire/HangFireSetup.cs:18-27).
  - Dashboard com autorização permissiva no Docker para facilitar acesso (backend/NfcTest/src/Nfc.Infra.HangFire/HangFireSetup.cs:37-47).
- Observabilidade:
  - Serilog + OpenTelemetry (export OTLP e logs para Seq) configurados em tempo de execução (`ObservabilitySetup`) e variáveis Docker (backend/NfcTest/src/Nfc.Api/appsettings.Docker.json:40-46; docker-compose.yml:139).

## Armazenamento de Exportação
- Estratégia: `S3` com fallback para `Local`.
- Registro via DI: escolhe `S3` e cai para `Local` quando houver falha (backend/NfcTest/src/Nfc.Infra.Storage/ServiceCollectionExtensions.cs:12-31).
- Ajuste crítico de stream: para evitar `ObjectDisposedException`, o conteúdo é copiado para um `byte[]` e usado em `MemoryStream` independentes para primário/fallback (backend/NfcTest/src/Nfc.Infra.Storage/FallbackExportFileStorage.cs:17-32).

## Decisões Técnicas
- Hangfire no Docker com Redis como storage para filas e estados.
- Dashboard do Hangfire exposto em `/hangfire` com autorização permissiva no ambiente Docker para desenvolvimento (backend/NfcTest/src/Nfc.Infra.HangFire/HangFireSetup.cs:37-47).
- Exportação assíncrona com agendamento (scheduler) e notificação em tempo real via SignalR (`ExportStatusUpdated`) (backend/NfcTest/src/Nfc.Api/Notifications/SignalRExportStatusNotifier.cs:13-19; backend/NfcTest/src/Nfc.Api/Hubs/ExportStatusHub.cs:5-16).
- Observabilidade integrada: logs estruturados (Seq) e métricas/traces (OTLP + Prometheus/Grafana).
- Frontend Nginx faz proxy apenas para `/api/`, `/hubs/` e `/scalar/` (frontend/NfcTest/nginx.conf:24-46).

## Passo a passo para rodar localmente (backend, frontend, banco)
- Banco de dados e dependências via Docker:
  - `docker compose up -d nfc.mvp nfc.redis localstack aws-cli`
  - Isso sobe SQL Server (`1433`), Redis (`6379`) e prepara o bucket S3 no LocalStack.
- Backend sem Docker:
  - `dotnet build backend/NfcTest/NfcTest.sln -c Release`
  - `set ASPNETCORE_ENVIRONMENT=Development`
  - `dotnet run --project backend/NfcTest/src/Nfc.Api/Nfc.Api.csproj`
  - API disponível em `http://localhost:5078` (ou porta exibida pelo run). Para alinhar com Docker, pode usar `ASPNETCORE_URLS=http://localhost:8085`.
- Frontend sem Docker:
  - `cd frontend/NfcTest`
  - `npm install`
  - `npm run start`
  - Dev server (Angular) disponível em `http://localhost:4200`. Configure as chamadas para a API em `http://localhost:8085`.
- Alternativa: subir tudo com Docker
  - `docker compose up -d`
  - Acesse: API `http://localhost:8085`, Front `http://localhost:8082`, Hangfire `http://localhost:8085/hangfire`.

## Suas escolhas técnicas e por quê
- Por que escolheu o Front-end?
  - Angular oferece CLI madura, tipagem com TypeScript, RxJS para tempo real (SignalR), e build estável para deploy via Nginx. O projeto já contém estrutura Angular e `nginx.conf` com proxys para API e hubs (frontend/NfcTest/nginx.conf:24-46).
- Por que a fila? (ou por que usou RabbitMQ?)
  - A exportação é tarefa pesada/assíncrona; usamos Hangfire com Redis para fila e persistência de jobs, evitando bloquear requisições HTTP e permitindo retries, monitoramento e deduplicação (backend/NfcTest/src/Nfc.Infra.HangFire/HangFireSetup.cs:18-27; backend/NfcTest/src/Nfc.Infra.HangFire/Jobs/ExportScheduler.cs:32-75, 90-111).
  - RabbitMQ é excelente como broker entre serviços; aqui priorizamos simplicidade e integração nativa .NET. Em cenário de microserviços, RabbitMQ/SQS seria considerado para desacoplar produtores/consumidores.
- Por que EF Core ou Dapper?
  - EF Core foi escolhido pela produtividade, LINQ e migrações, mantendo bom equilíbrio de performance e manutenibilidade (backend/NfcTest/src/Nfc.Infra.Data.EF). Dapper seria opção para queries críticas/relatórios de alto volume.
- Como você lidaria com falhas em produção?
  - Isolação por correlação (`CorrelationId`), fallback de storage (S3 → Local) já implementado (backend/NfcTest/src/Nfc.Infra.Storage/FallbackExportFileStorage.cs:17-32), monitoramento via Hangfire Dashboard e alertas (Seq/OTLP/Prometheus).

## Endpoints Principais
- Nota Fiscal (REST):
  - `POST /api/notafiscal` cria uma nota (backend/NfcTest/src/Nfc.Api/Controllers/NotaFiscalController.cs:26-36).
  - `GET /api/notafiscal` lista (backend/NfcTest/src/Nfc.Api/Controllers/NotaFiscalController.cs:39-47).
  - `GET /api/notafiscal/{id}` obtém por id (backend/NfcTest/src/Nfc.Api/Controllers/NotaFiscalController.cs:50-58).
  - `PUT /api/notafiscal/{id}` atualiza (backend/NfcTest/src/Nfc.Api/Controllers/NotaFiscalController.cs:60-77).
  - `DELETE /api/notafiscal/{id}` remove (backend/NfcTest/src/Nfc.Api/Controllers/NotaFiscalController.cs:80-87).
- Exportação:
  - `POST /api/export` agenda exportação (backend/NfcTest/src/Nfc.Api/Controllers/ExportController.cs:23-30).
  - `GET /api/export/status/{jobId}` consulta status (backend/NfcTest/src/Nfc.Api/Controllers/ExportController.cs:32-39).
  - `GET /api/export/file/{jobId}` baixa arquivo (backend/NfcTest/src/Nfc.Api/Controllers/ExportController.cs:41-51).

## Exemplos
- Criar Nota Fiscal:
  - `curl -X POST http://localhost:8085/api/notafiscal -H "Content-Type: application/json" -d "{ \"Emissor\": \"ACME\", \"DataEmissao\": \"2025-11-22T00:00:00Z\", \"Items\": [ { \"Descricao\": \"Produto A\", \"Valor\": 10.5 }, { \"Descricao\": \"Produto B\", \"Valor\": 20 } ] }"`
- Exportar Notas Fiscais:
  - `curl -X POST http://localhost:8085/api/export -H "Content-Type: application/json" -d "{ \"type\": \"JSON\", \"ids\": [1,2,3] }"`
  - resposta esperada: `202 Accepted` com `jobId` e `correlationId`.
- Consultar Status:
  - `curl http://localhost:8085/api/export/status/<jobId>`
- Baixar Arquivo:
  - `curl -OJ http://localhost:8085/api/export/file/<jobId>`
- SignalR (browser):
  - Conectar: `new signalR.HubConnectionBuilder().withUrl("http://localhost:8082/hubs/export-status").build()`
  - Entrar no grupo do job: `connection.invoke("JoinJobGroup", jobId)`
  - Receber atualizações: `connection.on("ExportStatusUpdated", s => console.log(s))`
- Hangfire Dashboard:
  - `http://localhost:8085/hangfire`

### Respostas esperadas (exemplos)
- `POST /api/export` (ExportNotaFiscalCommand)
  - Request: `{ "type": "JSON", "ids": [1,2,3] }` (backend/NfcTest/src/Nfc.Application/UseCases/Export/ExportNotaFiscal/ExportNotaFiscalCommand.cs:6-10)
  - Response: `202 Accepted`
    ```json
    { "jobId": "<id-gerado>", "correlationId": "<guid>" }
    ```
- `GET /api/export/status/{jobId}`
  - Response: `200 OK`
    ```json
    {
      "jobId": "<id>",
      "correlationId": "<guid>",
      "state": "Queued|Started|Completed|Failed",
      "type": "JSON|TXT",
      "ids": [1,2,3],
      "durationMs": 1234.5,
      "error": null,
      "fileUrl": null
    }
    ```
- `GET /api/export/file/{jobId}`
  - `200 OK` com conteúdo e `Content-Type` conforme tipo (backend/NfcTest/src/Nfc.Api/Controllers/ExportController.cs:41-51).

## Execução local sem Docker (opcional)
- Backend:
  - `dotnet build backend/NfcTest/NfcTest.sln -c Release`
  - `dotnet run --project backend/NfcTest/src/Nfc.Api/Nfc.Api.csproj`
- Frontend:
  - `cd frontend/NfcTest`
  - `npm install && npm run start`

## Resolução de Problemas
- Acesso ao Hangfire retorna `401`:
  - No Docker, o acesso externo não é considerado "local"; a autorização permissiva foi adicionada para facilitar desenvolvimento (backend/NfcTest/src/Nfc.Infra.HangFire/HangFireSetup.cs:37-47).
- Falha ao exportar por `ObjectDisposedException`:
  - Corrigido via buffer e `MemoryStream` independentes no fallback (backend/NfcTest/src/Nfc.Infra.Storage/FallbackExportFileStorage.cs:17-32).

## Perguntas de julgamento técnico — respostas
1. Evitar timeout no frontend para job de 10 minutos
   - Responder imediatamente com `202 Accepted` e `jobId` (backend/NfcTest/src/Nfc.Api/Controllers/ExportController.cs:23-30), acompanhar progresso por SignalR (`/hubs/export-status`) ou polling em `GET /api/export/status/{jobId}`. O download é separado em `GET /api/export/file/{jobId}`; assim, nenhuma chamada HTTP única fica bloqueada muitos minutos.
2. Garantir idempotência
   - Deduplicação por chave determinística e lock distribuído em Redis antes de enfileirar (backend/NfcTest/src/Nfc.Infra.HangFire/Jobs/ExportScheduler.cs:44-75, 94-111). A chave `export:dedup:{type}:{ids-ordenados}` expira após 1h (backend/NfcTest/src/Nfc.Infra.HangFire/Jobs/ExportScheduler.cs:20, 125-129). Se a mesma requisição chegar enquanto houver chave, retorna o `jobId` já existente.
3. Cache em cenário com milhares de clientes
   - Distribuído (Redis) para escalar horizontalmente e compartilhar cache entre instâncias. `In-memory` só serve para estado efêmero por processo; em produção com múltiplas réplicas, causa inconsistências.
4. Integração ao legado .NET 2.2
   - Expor o módulo via API REST estável e/ou mensageria (SQS/RabbitMQ) e manter contratos em assemblies `netstandard2.0`.integrar via Dashboard separado ou consumir filas do Redis em worker compatível.
5. Necessidade de fila neste MVP
   - Sim, para exportações pesadas e não-interativas, liberando o request e permitindo controle (retry/cancel/monitor). Cenário de valor: export de grande volume, agregações demoradas ou dependências externas (S3), evitando timeouts e impactar UX.
6. Suportar picos de 10x
   - Escalar horizontalmente API e workers, ajustar `WorkerCount` do Hangfire, usar backpressure limitar concorrência, usar cache para leituras, pré-agendar fora do horário de pico. Garantir que storage (S3/Local) sustente I/O.
7. Ponto mais frágil hoje e resiliência
   - Dependência única do Redis para deduplicação, lock e status. Em partições/restart, pode ocorrer duplicidade de jobs e perda de estado. Mitigar persistindo idempotência/estado em SQL com índice único (ou usando Hangfire.SqlServer) e RedLock para lock distribuído.
8. Observabilidade além de logs estruturados
   - Métricas (Prometheus), traces distribuídos (OTLP/OpenTelemetry),health checks e alertas (SLO/SLI).
9. Priorizar performance ou manutenibilidade?
   - Manutenibilidade neste contexto MVP: permite evoluir rápido e reduzir risco.
10. Entregar para outra equipe manter
   - Adicionar testes de integração para exportações, documentar contratos e fluxos, automatizar pipelines (CI/CD), parametrizar todos os caminhos via `appsettings`.

## Referências
- https://medium.com/@serasiyasavan14/mediatr-pipeline-behavior-0e2445daac4a
- https://github.com/helberthlucas14/FC.Codeflix.Catalog
- https://github.com/helberthlucas14/product_registry
- https://medium.com/@mahmednisar/logging-like-a-pro-serilog-opentelemetry-in-net-3c9f219b9296