# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**WingDing Party** — микросервисная платформа для организации тематических встреч и клубных сообществ. Написана на .NET 10, использует Keycloak (OIDC), PostgreSQL, Redis, Apache Kafka.

## Knowledge Graph

В репозитории может существовать граф знаний в `graphify-out/`:

- `graphify-out/graph.json` — машиночитаемый граф (узлы, рёбра, сообщества)
- `graphify-out/GRAPH_REPORT.md` — отчёт: god nodes, неожиданные связи, предложенные вопросы
- `graphify-out/graph.html` — интерактивная визуализация

**Перед тем как читать файлы исходного кода**, проверь наличие графа:

```powershell
Test-Path graphify-out/graph.json
```

Если граф существует — используй его для навигации по архитектуре, поиска зависимостей и понимания структуры. Это экономит контекст: вместо чтения десятков файлов достаточно запросить нужный узел или путь в графе. Обращайся к исходникам только для деталей реализации, которых нет в графе.

Граф можно пересобрать или обновить командой `/graphify src/<ServiceName>` (incremental: `/graphify src/<ServiceName> --update`).

## Commands

### Build & Run

```powershell
# Собрать весь solution
dotnet build WingDing-Party.sln

# Собрать один сервис
dotnet build src/AuthService/AuthService.sln

# Запустить сервис локально
dotnet run --project src/AuthService/AuthService.Api

# Запустить инфраструктуру (Keycloak, Postgres, Redis, Kafka, pgAdmin)
docker compose up -d

# Запустить только конкретный контейнер
docker compose up postgres redis keycloak -d
```

### EF Core Migrations

```powershell
# Создать миграцию (пример для AuthService)
dotnet ef migrations add <MigrationName> `
  --project src/AuthService/AuthService.Infrastructure `
  --startup-project src/AuthService/AuthService.Api

# Применить миграции вручную
dotnet ef database update `
  --project src/AuthService/AuthService.Infrastructure `
  --startup-project src/AuthService/AuthService.Api
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

### Авторизация (permission-based)

Система разрешений поверх стандартного ASP.NET Core Authorization:

1. **`[HasPermission("events:create")]`** — атрибут на endpoint (создаёт динамическую policy)
2. **`PermissionAuthorizationPolicyProvider`** — превращает имя permission в policy
3. **`PermissionAuthorizationHandler`** — проверяет claim `permission` у пользователя
4. **`CustomClaimsTransformation`** — после валидации JWT, обогащает ClaimsPrincipal данными из нашей БД: заменяет Keycloak `sub` на внутренний `User.Id`, добавляет роли из PostgreSQL
5. **`AuthorizationService`** — запрашивает роли/permissions из БД, кеширует в Redis (TTL 5 мин, ключ `auth:roles-{identityId}`)

Разрешения определены как static readonly поля в `Permission : Enumeration` (например, `Permission.EventsCreate`).

### Интеграция с Keycloak

- **`IAuthenticationService`** / `AuthenticationService` — регистрация пользователя через Keycloak Admin API (`/admin/realms/wingding-party/users`)
- **`IJwtService`** / `JwtService` — получение JWT токена через ROPG Flow (`/realms/wingding-party/protocol/openid-connect/token`)
- **`AdminAuthorizationDelegatingHandler`** — автоматически добавляет Bearer-токен admin-клиента к исходящим запросам к Admin API
- JWT Bearer настраивается через `JwtBearerOptionsSetup` (читает `Authentication` секцию из конфига)

### Конфигурация

Конфиг-классы биндятся на root configuration (не на секцию), то есть переменные окружения (`CONNECTION_STRING`, `REDIS_CONNECTION_STRING`) маппируются напрямую. Секции `Authentication` и `Keycloak` биндятся явно через `configuration.GetSection(...)`.

Порядок источников: `appsettings.json` → `appsettings.Development.json` → Environment Variables → User Secrets.

### Логирование

Serilog с раздельными файловыми синками:
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
