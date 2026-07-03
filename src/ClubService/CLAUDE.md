# ClubService

Клубы по интересам и членство. Агрегат `Club` (владелец, описание, интересы, публичность) и `ClubMember`. Порт API — **5400**.

## Knowledge Graph

Граф знаний **один на весь репозиторий** (все сервисы + SharedKernel) и лежит в `.claude/graphify-out/`. Отдельного графа под ClubService нет — см. дисциплину графа в [`.claude/CLAUDE.md`](../../.claude/CLAUDE.md).

**Перед чтением исходников** проверь граф: `Test-Path ../../.claude/graphify-out/graph.json`. Если есть — навигируй по нему, к файлам обращайся только за деталями, которых в графе нет. Пересобрать граф — всегда по всему `src/`: `/graphify src/ --update`.

## CQRS (ClubManagement)

`ClubManagement/Command` (Create/Update/Delete/JoinToClub/LeaveClub) и `ClubManagement/Queries` (GetClub, GetClubMembers, GetClubsByUser, SearchClubs). Хендлеры — `IRequestHandler`, валидация — `ValidationBehavior`. Доменный сервис — `IClubService`/`ClubService` (агрегирует репозиторий + членство). Запросы — через спецификации (`ClubByIdSpec`, `ClubsByUserSpec`, `SearchClubsSpec` и т.д., `BaseSpecification`).

## Messaging: Kafka (producer) + Outbox

Публикует интеграционные события о клубах (`IntegrationEvents/Clubs`):
- `IIntegrationEventDispatcher`/`KafkaIntegrationEventDispatcher`, `IKafkaProducerFactory`/`KafkaProducerFactory`, `IEventTypeMapper`/`EventTypeMapper`;
- Outbox через `OutboxProcessorBackgroundService`.

`KafkaOptions` биндится как `Dictionary<string, KafkaOptions>` (`GetSection(nameof(KafkaOptions))`, валидатор `KafkaOptionsValidator`) — тот же паттерн, что в UserService, и отличается от одиночной секции в EventService.

## Авторизация (remote, permission-based)

Downstream-сервис: `AddWingDingAuthCore()` + `AddWingDingAuthRemote(configuration)` из `WingDing.Auth.Shared` (SharedKernel). JWT Bearer поверх Keycloak (секция `Authentication`) + gRPC-клиент `PermissionOracle` к AuthService (:5200) + `GrpcPermissionService` (`IPermissionService`) с `IMemoryCache`. Эндпоинты защищаются `[HasPermission(...)]`; permissions-строки — в `WingDing.Auth.Shared/Permissions.cs`.

## Persistence

⚠️ **Gotcha именования:** DbContext ClubService называется `UserServiceDbContext` (класс в `ClubService.Infrastructure.Persistence.UserServiceDbContext`, не переименованный copy-paste из UserService). Это **не** контекст UserService — он маппит именно club-конфигурации и живёт в неймспейсе ClubService, но имя вводит в заблуждение (и снапшот миграций тоже `UserServiceDbContextModelSnapshot`). Кандидат на переименование в `ClubServiceDbContext` через новую миграцию.

Особенности регистрации: в отличие от Event/User, ClubService регистрирует **только** `AddDbContextFactory` (нет scoped `AddDbContext`). Репозиторий — `GenericRepository<Club, ClubId>`, `IUnitOfWork`. Миграции на старте (`app.ApplyMigrations()`).

## Конфигурация

Секции биндятся через `configuration.GetSection(Options.SectionName)`; строка подключения (`CONNECTION_STRING` в `ClubsDatabaseOptions`) — из `.env`. Options-классы: `ApiOptions`, `ClubsDatabaseOptions`, `KafkaOptions` (словарь).
