# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**WingDing Party** — микросервисная платформа для организации тематических встреч и клубных сообществ. Написана на .NET 10, использует Keycloak (OIDC), PostgreSQL, Redis, Apache Kafka.

## Claude Configuration

Claude Code–специфичная конфигурация (скиллы, Knowledge Graph, триггеры) — в [`.claude/CLAUDE.md`](.claude/CLAUDE.md).
Сервисная документация — в `src/<ServiceName>/CLAUDE.md`.

### Session bootstrap — где брать контекст (читать ДО анализа кода)

Не анализируй кодовую базу с нуля — сначала используй уже собранный контекст. Для любой архитектурной работы, планирования или задач по сервису:

1. **Граф знаний** — основной навигатор. Один общий граф на весь `src/` в `.claude/graphify-out/`. Сначала проверь scope и свежесть (заголовок `GRAPH_REPORT.md` + дата `graph.json`); если актуален — навигируй по `graph.json` / `GRAPH_REPORT.md`, к исходникам обращайся только за деталями. Если scope не тот или устарел — `/graphify src/ --update`. Детали дисциплины — в [`.claude/CLAUDE.md`](.claude/CLAUDE.md).
2. **Сервисные `CLAUDE.md`** — у каждого сервиса есть `src/<ServiceName>/CLAUDE.md` (Auth, User, Event, Club) с его специфм (messaging, storage, авторизация, конфиг). Прочитай нужный **до** чтения его исходников; они не подгружаются автоматически на старте сессии.
3. **`docs/`** — сквозная документация: [`docs/auth_doc.md`](docs/auth_doc.md) (аутентификация/авторизация) и [`docs/grpc_doc.md`](docs/grpc_doc.md) (gRPC `PermissionOracle` между сервисами). Сверяйся с ними, не выдумывай контракты.

## Commands

### Build & Run

```powershell
# Собрать весь solution
dotnet build WingDingParty.slnx

# Запустить сервис локально
dotnet run --project src/<ServiceName>/<ServiceName>.Api

# Запустить инфраструктуру (Keycloak, Postgres, Redis, Kafka, pgAdmin)
docker compose up -d

# Запустить только конкретный контейнер
docker compose up postgres redis keycloak -d
```

### EF Core Migrations

```powershell
# Создать миграцию
dotnet ef migrations add <MigrationName> `
  --project src/<ServiceName>/<ServiceName>.Infrastructure `
  --startup-project src/<ServiceName>/<ServiceName>.Api

# Применить миграции вручную
dotnet ef database update `
  --project src/<ServiceName>/<ServiceName>.Infrastructure `
  --startup-project src/<ServiceName>/<ServiceName>.Api
```

Миграции применяются **автоматически** при запуске через `app.ApplyMigrations()` в `Program.cs`.

## Local Infrastructure Ports

| Сервис | Порт |
|---|---|
| AuthService API | 5200 |
| UserService API | 5100 |
| EventService API | 5300 |
| ClubService API | 5400 |
| Keycloak | 18080 |
| PostgreSQL | 5432 |
| Redis | 6379 |
| Kafka | 9092 |
| Kafka UI | 8090 |
| pgAdmin | 5050 |

Keycloak realm: `wingding-party`. Конфигурация realm импортируется при старте из `.files/`.

## Architecture

### Структура монорепозитория

```
src/
  AuthService/    # Аутентификация и авторизация
  UserService/    # Профили пользователей
  EventService/   # Управление событиями
  ClubService/    # Клубы по интересам
```

Каждый сервис — отдельный solution (`*.sln`) и следует **Clean Architecture**:

```
<Service>.Api            # Presentation: DI-регистрация, Minimal API endpoints, Swagger
<Service>.Application    # Use Cases: интерфейсы сервисов, CQRS-команды/запросы
<Service>.Domain         # Domain: сущности, value objects, перечисления (нет зависимостей)
<Service>.Infrastructure # Persistence: EF Core, внешние HTTP-клиенты, Redis, DI-реализации
<Service>.Contracts      # Shared DTOs для межсервисной коммуникации
```

### DDD паттерны в Domain

- **Entity\<TId\>** — базовый класс с identity-based equality (см. `Domain/Common/Abstractions/Entity.cs`)
- **ValueObject** — equality по компонентам через `GetEqualityComponents()`
- **Enumeration** — типобезопасные перечисления вместо `enum` (id + name + description)
- Все ID — строго типизированные value objects: `UserId`, `RoleId` и т.д.
- Фабричный метод `Create(...)` вместо публичных конструкторов

### Регистрация зависимостей

Каждый слой регистрирует себя через extension-методы в `DependencyInjection.cs`:

```csharp
// Program.cs
builder.Services
    .AddPresentation(builder.Configuration)  // Api layer
    .AddInfrastructure(builder.Configuration) // Infrastructure layer
    .AddApplication();                        // Application layer
```

### Конфигурация

Большинство опций биндятся на **именованные секции** через `configuration.GetSection(Options.SectionName)` — у каждого options-класса есть константа `SectionName`. Часть инфраструктурных опций (например, БД и Redis в AuthService) биндится напрямую на **root** через `configuration.Bind`, чтобы переменные окружения маппировались без секции. Порядок источников: `appsettings.json` → `appsettings.Development.json` → Environment Variables → User Secrets.

Сервис-специфичные секции конфигурации описаны в `src/<ServiceName>/CLAUDE.md`.

### Логирование

Serilog с раздельными файловыми синками (настраивается в `appsettings.json` каждого сервиса):
- `../logs/Information/log-*.txt`
- `../logs/Warning/log-*.txt`
- `../logs/Error/log-*.txt`

## Code Style

Настроен через `.editorconfig`:
- **Нет `var`** — всегда явные типы (`csharp_style_var_*= false`)
- **File-scoped namespaces** (`namespace Foo.Bar;`)
- **Primary constructors** — предпочтительны для DI
- Отступ 4 пробела, CRLF
- Интерфейсы с префиксом `I`, типы и члены в PascalCase
