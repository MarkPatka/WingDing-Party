# UserService

Профили пользователей и аватары. Хранит `UserProfile` (данные профиля, интересы) и `Avatar`-изображения в объектном хранилище MinIO. Порт API — **5100**.

## Knowledge Graph

Граф знаний **один на весь репозиторий** (все сервисы + SharedKernel) и лежит в `.claude/graphify-out/`. Отдельного графа под UserService нет — см. дисциплину графа в [`.claude/CLAUDE.md`](../../.claude/CLAUDE.md).

**Перед чтением исходников** проверь граф: `Test-Path ../../.claude/graphify-out/graph.json`. Если есть — навигируй по нему, к файлам обращайся только за деталями, которых в графе нет. Пересобрать граф — всегда по всему `src/`: `/graphify src/ --update`.

## CQRS (UserProfileManagement, AvatarManagement)

Две группы фич: `UserProfileManagement/Command` + `Queries` (профиль, интересы) и `AvatarManagement/Commands` (создание/обновление/удаление аватара). Хендлеры — `IRequestHandler`, валидация — `ValidationBehavior` в `Application/Common/Behaviors`. Доменный сервис — `IUserProfileService`/`UserProfileService`.

## Хранилище аватаров: MinIO

Отдельная от всех подсистема (`Infrastructure/Storage`):
- `IMinioClient` (конфигурируется из `FileStorageOptions`: `Endpoint`, `AccessKey`, `SecretKey`, `WithSsl`);
- `IMinioBucketManager`/`MinioBucketManager` — управление бакетами;
- `IFileStorage`/`MinioFileStorage` — загрузка/удаление файлов аватаров.

Все три — синглтоны. Секция конфигурации — `FileStorage`.

## Messaging: Kafka (producer) + Outbox

UserService **публикует** интеграционные события об изменении профиля (их слушает EventService — `UserProfileUpdatedIntegrationEventHandler`):
- `IIntegrationEventDispatcher`/`KafkaIntegrationEventDispatcher`, `IKafkaProducerFactory`/`KafkaProducerFactory`;
- `IEventTypeMapper`/`EventTypeMapper` и `IIntegrationEventTypeResolver` — маппинг типов событий (Mapster-конфиги в `IntegrationEvents/Mapping`);
- Outbox через `OutboxProcessorBackgroundService`.

**Важно:** `KafkaOptions` здесь биндится как `Dictionary<string, KafkaOptions>` (несколько именованных конфигов, `GetSection(nameof(KafkaOptions))`) — не как одиночная секция, в отличие от EventService. Валидатор — `KafkaOptionsValidator` над словарём. Выделенного consumer-hosted-service нет (сервис — продюсер).

## Авторизация (remote, permission-based)

Downstream-сервис: `AddWingDingAuthCore()` + `AddWingDingAuthRemote(configuration)` из `WingDing.Auth.Shared` (SharedKernel). JWT Bearer поверх Keycloak (секция `Authentication`) + gRPC-клиент `PermissionOracle` к AuthService (:5200) + `GrpcPermissionService` (`IPermissionService`) с `IMemoryCache`. Эндпоинты защищаются `[HasPermission(...)]`; permissions-строки — в `WingDing.Auth.Shared/Permissions.cs`.

## Persistence

`UserServiceDbContext` (Npgsql, `EnableRetryOnFailure(2)`, legacy timestamp). Регистрируются scoped `AddDbContext` и `AddDbContextFactory` (фабрика — для outbox background service). Репозиторий — `GenericRepository<UserProfile, UserId>`, `IUnitOfWork`. Запросы — спецификации (`Persistence/Specifications`). Миграции на старте (`app.ApplyMigrations()`).

## Конфигурация

Секции биндятся через `configuration.GetSection(Options.SectionName)`; строка подключения (`CONNECTION_STRING` в `UserDatabaseOptions`) — из `.env`. Options-классы: `ApiOptions`, `FileStorageOptions` (секция `FileStorage`), `KafkaOptions` (словарь), `UserDatabaseOptions`.
