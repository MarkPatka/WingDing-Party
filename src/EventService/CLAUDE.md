# EventService

Управление событиями (events) платформы: создание/обновление/отмена событий, типы событий, регистрация участников, рейтинги. Самый крупный сервис (≈188 .cs). Порт API — **5300**.

## Knowledge Graph

Граф знаний **один на весь репозиторий** (все сервисы + SharedKernel) и лежит в `.claude/graphify-out/`. Отдельного графа под EventService нет — см. дисциплину графа в [`.claude/CLAUDE.md`](../../.claude/CLAUDE.md).

**Перед чтением исходников** проверь граф: `Test-Path ../../.claude/graphify-out/graph.json`. Если есть — навигируй по нему, к файлам обращайся только за деталями, которых в графе нет. Пересобрать граф — всегда по всему `src/`: `/graphify src/ --update`.

## CQRS (EventManagement)

`EventManagement/Command` и `EventManagement/Queries` — команды/запросы через MediatR-подобные хендлеры (`IRequestHandler`). Валидация — через `ValidationBehavior` в `Application/Common/Behaviors`. Доменные сервисы (`IEventService`, `IEventTypeService`) — в `Infrastructure/Services`.

## Messaging: Kafka + Transactional Outbox

Самая насыщенная messaging-подсистема из всех сервисов (`Application/EventSourcing`, `Infrastructure/EventSourcing`):

- **Domain events** → `EventSourcing/DomainEventHandlers` (`EventCreated/Updated/Cancelled/Deleted/Published`, `ParticipantRegistered`).
- **Transactional Outbox** — доменные события пишутся в таблицу outbox в той же транзакции (`IOutboxService`, `OutboxProcessor`), затем `OutboxProcessorBackgroundService` публикует их в Kafka. Настройки — `OutboxOptions` (+ `OutboxOptionsValidator`).
- **Producer side**: `IEventProducer`/`KafkaEventProducer`, `IIntegrationEventPublisher`/`KafkaIntegrationEventPublisher`, `IDeadLetterQueueProducer`/`DeadLetterQueueProducer` (DLQ для несходящихся сообщений).
- **Consumer side**: `IEventConsumer`/`KafkaEventConsumer` + `KafkaEventConsumerBackgroundService` (hosted service). Входящие события обрабатываются `IIntegrationEventHandler<T>` — например, `UserProfileUpdatedIntegrationEventHandler` (слушает UserService).
- **Типизация событий**: `IIntegrationEventTypeRegistry` / `IIntegrationEventDispatcher`.
- **Топики**: `ITopicProvisioner`/`KafkaTopicProvisioner` — топики создаются на старте через `app.ProvisionKafkaTopics()` в `Program.cs`.

`KafkaOptions` биндится как **одиночная секция** (`GetSection("KafkaOptions")`, `ValidateOnStart`) — в отличие от User/Club, где это `Dictionary<string, KafkaOptions>`.

## Авторизация (remote, permission-based)

EventService — downstream-сервис: `AddWingDingAuthCore()` + `AddWingDingAuthRemote(configuration)` из `WingDing.Auth.Shared` (SharedKernel). Это даёт:
- JWT Bearer поверх Keycloak (секция `Authentication`: `Audience`, `MetadataUrl`, `Issuer`, `RequireHttpsMetadata`);
- gRPC-клиент `PermissionOracle.PermissionOracleClient` к AuthService (`AuthServiceGrpcUrl`, дефолт `http://auth-service:5200`);
- `GrpcPermissionService` как `IPermissionService` — проверка прав удалённо у AuthService, с `IMemoryCache`.

Эндпоинты защищаются атрибутом `[HasPermission("events:create")]` (плумбинг из SharedKernel). Права/permissions-строки — в `WingDing.Auth.Shared/Permissions.cs`.

## Persistence

`EventServiceDbContext` (Npgsql, `EnableRetryOnFailure(2)`, legacy timestamp behavior). Регистрируются и scoped `AddDbContext`, и `AddDbContextFactory` (фабрика нужна для фоновых сервисов outbox/consumer). Репозитории — `GenericRepository<Event, EventId>`, `GenericRepository<EventType, EventTypeId>`, `IUnitOfWork`. Запросы — через спецификации (`Persistence/Specifications`, `BaseSpecification`). Миграции применяются на старте (`app.ApplyMigrations()`).

## Конфигурация

Секции биндятся через `configuration.GetSection(Options.SectionName)`; строка подключения БД (`CONNECTION_STRING`) и прочие `EventsDatabaseOptions` собираются из `.env` (через `MemoryConfigurationSource` в `AddConfiguration`). Options-классы: `ApiSettings`, `EventsDatabaseOptions`, `KafkaOptions`, `OutboxOptions`, `PgAdminSettings` — у каждого константа `SectionName`.
