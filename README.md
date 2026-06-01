# 🎉 WingDing Party

> Микросервисная платформа для организации тематических встреч, знакомств по интересам и общения внутри закрытого клубного сообщества.

**WingDing Party** — это набор независимо разрабатываемых и развёртываемых микросервисов на **.NET 10**, объединённых вокруг единого сценария: помочь людям со схожими интересами находить друг друга, создавать события (квартирники, походы в театр, бар, кино, мастер-классы) и объединяться в клубы. Платформа построена на принципах **Clean Architecture**, **Domain-Driven Design** и **CQRS**, использует асинхронную коммуникацию через **Apache Kafka** и синхронную через **REST/gRPC**, а вопросы идентификации решает через **Keycloak (OIDC)**.

<!-- TODO: сюда добавить логотип / баннер проекта -->

---

## 📑 Оглавление

- [Цели проекта](#-цели-проекта)
- [Возможности](#-возможности)
- [Технологии](#-технологии)
- [Архитектура](#-архитектура)
  - [Clean Architecture](#clean-architecture)
  - [Domain-Driven Design](#domain-driven-design)
  - [CQRS](#cqrs)
- [Микросервисы](#-микросервисы)
  - [AuthService](#authservice--аутентификация-и-авторизация)
  - [UserService](#userservice--профили-пользователей)
  - [EventService](#eventservice--события)
  - [ClubService](#clubservice--клубы-по-интересам)
- [Бизнес-сервисы: единый архитектурный шаблон](#-бизнес-сервисы-единый-архитектурный-шаблон)
- [Межсервисное взаимодействие](#-межсервисное-взаимодействие)
- [Структура решения](#-структура-решения)
- [Как запустить](#-как-запустить)
- [Порты и инфраструктура](#-порты-и-инфраструктура)
- [Дорожная карта](#-дорожная-карта)
- [Лицензия](#-лицензия)

---

## ❇️ Цели проекта

Проект задуман как учебно-практическая площадка для отработки построения зрелой микросервисной системы. Ключевые цели:

- ❇️ Построить **отказоустойчивую микросервисную архитектуру** на .NET 10 с независимым развёртыванием сервисов.
- ❇️ Применить **Clean Architecture** на каждом сервисе с чётким разделением слоёв (Api / Application / Domain / Infrastructure / Contracts).
- ❇️ Использовать тактические паттерны **Domain-Driven Design** — агрегаты, value objects, строго типизированные идентификаторы, типобезопасные перечисления.
- ❇️ Реализовать **CQRS** на основе MediatR с разделением команд и запросов.
- ❇️ Вынести идентификацию во внешний **Identity Provider (Keycloak)** по протоколу OIDC и построить собственную **permission-based авторизацию** поверх ASP.NET Core.
- ❇️ Совместить **синхронную (REST + gRPC)** и **асинхронную (Kafka)** межсервисную коммуникацию.
- ❇️ Обеспечить воспроизводимое локальное окружение «одной командой» через **Docker Compose**.

---

## ✔️ Возможности

Реализованный и планируемый функционал платформы:

- ✔️ Регистрация и аутентификация пользователей через Keycloak (OIDC, JWT).
- ✔️ Ролевая модель и тонкая **авторизация по разрешениям** (`events:create`, `clubs:manage` и т.п.).
- ✔️ Управление профилями пользователей и загрузка аватаров в S3-совместимое хранилище (MinIO).
- ✔️ Создание и управление событиями различных типов.
- ✔️ Клубы по интересам: создание, вступление, управление участниками.
- ✔️ Асинхронные доменные события между сервисами через Kafka.
- ✔️ gRPC-контракт `PermissionOracle` для синхронного получения ролей и разрешений другими сервисами.
- 🚧 Система рекомендаций событий на основе интересов *(в планах)*.
- 🚧 Real-time чат и уведомления *(в планах)*.
- 🚧 Полнотекстовый поиск *(в планах)*.

---

## 🛠 Технологии

| Технология | Назначение |
|---|---|
| ✔️ [.NET 10](https://dotnet.microsoft.com/) | Платформа всех сервисов (LTS) |
| ✔️ [ASP.NET Core](https://learn.microsoft.com/aspnet/core/) | REST API, Minimal API, middleware |
| ✔️ [Entity Framework Core 10](https://learn.microsoft.com/ef/core/) + [Npgsql](https://www.npgsql.org/) | ORM поверх PostgreSQL, миграции |
| ✔️ [Dapper](https://github.com/DapperLib/Dapper) | Лёгкие read-запросы там, где не нужен EF |
| ✔️ [PostgreSQL](https://www.postgresql.org/) | Транзакционное хранилище (отдельная БД на сервис) |
| ✔️ [Redis](https://redis.io/) | Кеширование ролей/разрешений и сессий |
| ✔️ [Apache Kafka](https://kafka.apache.org/) ([Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet)) | Асинхронная событийная коммуникация |
| ✔️ [gRPC](https://grpc.io/) ([Grpc.AspNetCore](https://github.com/grpc/grpc-dotnet)) | Синхронные межсервисные вызовы |
| ✔️ [Keycloak](https://www.keycloak.org/) | Identity Provider (OIDC, JWT) |
| ✔️ [MinIO](https://min.io/) | S3-совместимое хранилище медиафайлов |
| ✔️ [MediatR](https://github.com/jbogard/MediatR) | Реализация CQRS (команды/запросы) |
| ✔️ [Mapster](https://github.com/MapsterMapper/Mapster) | Маппинг DTO ↔ доменные модели |
| ✔️ [FluentValidation](https://docs.fluentvalidation.net/) | Валидация входных запросов |
| ✔️ [Serilog](https://serilog.net/) | Структурированное логирование с раздельными синками |
| ✔️ [Swashbuckle / Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) | OpenAPI-документация |
| ✔️ [Docker Compose](https://docs.docker.com/compose/) | Локальное окружение и инфраструктура |

> Версии пакетов управляются централизованно через `Directory.Packages.props` (Central Package Management).

---

## 🏛 Архитектура

Платформа — это **монорепозиторий**, в котором каждый микросервис представляет собой отдельный solution (`*.sln`) и развивается независимо. Сервисы общаются между собой асинхронно (Kafka) и синхронно (REST/gRPC), не разделяя баз данных — у каждого сервиса своя БД (`authdb`, `userdb`, `eventdb`, `clubdb`).

### Clean Architecture

Каждый сервис разделён на пять слоёв с однонаправленными зависимостями (внутрь, к домену):

```
<Service>.Api            # Presentation: DI-регистрация, контроллеры/Minimal API, Swagger, middleware
<Service>.Application    # Use Cases: интерфейсы сервисов, CQRS-команды и запросы, валидация
<Service>.Domain         # Domain: сущности, value objects, перечисления (без внешних зависимостей)
<Service>.Infrastructure # Persistence: EF Core, внешние HTTP/gRPC-клиенты, Redis, реализации сервисов
<Service>.Contracts      # Shared DTO для межсервисной коммуникации
```

Регистрация зависимостей собирается в композиционном корне `Program.cs` через extension-методы каждого слоя:

```csharp
builder.Services
    .AddPresentation(builder.Configuration)   // Api
    .AddInfrastructure(builder.Configuration) // Infrastructure
    .AddApplication();                        // Application
```

### Domain-Driven Design

В доменном слое последовательно применяются тактические паттерны DDD (на примере `AuthService`):

- **`Entity<TId>`** — базовый класс с identity-based equality.
- **`ValueObject`** — equality по компонентам через `GetEqualityComponents()`.
- **`Enumeration`** — типобезопасные перечисления вместо `enum` (например, `Permission`, `RoleType`).
- **Строго типизированные идентификаторы** — `UserId`, `RoleId` как value objects вместо «голых» `Guid`.
- **Фабричные методы `Create(...)`** вместо публичных конструкторов.

### CQRS

Слой Application построен на **MediatR**: входящие запросы превращаются в команды и запросы, которые обрабатываются отдельными хендлерами. Контроллеры тонкие — они лишь отправляют запрос через `ISender` и маппят результат через **Mapster** (`IMapper`). Валидация входных данных выполняется через **FluentValidation** (с единым `ValidationExceptionHandler`).

---

## 🧩 Микросервисы

| Сервис | Порт | Ответственность | Хранилище | Интеграции            |
|---|---|---|---|-----------------------|
| **AuthService** | 5200 | Аутентификация, авторизация, роли, разрешения | PostgreSQL, Redis | Keycloak, Kafka, gRPC |
| **UserService** | 5100 | Профили пользователей, аватары | PostgreSQL, MinIO | Kafka                 |
| **EventService** | 5300 | События, регистрации | PostgreSQL | Kafka                 |
| **ClubService** | 5400 | Клубы по интересам, участники | PostgreSQL | Kafka                 |

### AuthService — аутентификация и авторизация

Отвечает за регистрацию пользователей через Keycloak, выдачу JWT-токенов, роли и разрешения.

**Интеграция с Keycloak:**

- `IAuthenticationService` / `AuthenticationService` — регистрация пользователя через **Keycloak Admin API**.
- `IJwtService` / `JwtService` — получение JWT-токена по **Resource Owner Password Credentials (ROPC) Flow**.
- `AdminAuthorizationDelegatingHandler` — автоматически добавляет Bearer-токен admin-клиента к исходящим запросам к Admin API.
- `JwtBearerOptionsSetup` — настройка валидации входящих JWT (issuer, audience, metadata).

**Авторизация по разрешениям (permission-based):** конвейер поверх стандартной ASP.NET Core Authorization:

1. **`[HasPermission("events:create")]`** — атрибут на endpoint, создающий динамическую policy.
2. **`PermissionAuthorizationPolicyProvider`** — превращает имя разрешения в policy.
3. **`PermissionAuthorizationHandler`** — проверяет claim `permission` у пользователя.
4. **`CustomClaimsTransformation`** — после валидации JWT обогащает `ClaimsPrincipal`: заменяет Keycloak `sub` на внутренний `User.Id` и добавляет роли из PostgreSQL.
5. **`AuthorizationService`** — запрашивает роли/разрешения из БД и **кеширует их в Redis** (TTL 5 мин, ключ `auth:roles-{identityId}`).

**gRPC `PermissionOracle`:** AuthService предоставляет gRPC-контракт (`GetRoles`, `GetPermissions`), через который другие сервисы синхронно получают роли и разрешения пользователя без обращения к собственной БД.

**REST API (`UsersController`):** `Register`, `Login`, `GetMe`.

### UserService — профили пользователей

Управление профилями: отображаемое имя, краткая информация, интересы, аватар. Аватары хранятся в **MinIO** (S3-совместимое хранилище, бакет `avatars`). Публикует доменные события в Kafka (топик `userprofile`).

### EventService — события

Создание и управление событиями (квартирники, театр, бар, кино, мастер-классы), регистрации участников. Использует контроллеры, middleware и `ValidationExceptionHandler` для обработки ошибок валидации.

### ClubService — клубы по интересам

Создание клубов, вступление/выход, управление участниками. Публикует события в Kafka (топик `club`).

---

## 🧱 Бизнес-сервисы: единый архитектурный шаблон

**UserService**, **EventService** и **ClubService** — это «бизнесовые» сервисы платформы. В отличие от `AuthService` (который специфичен из-за интеграции с Keycloak и permission-based авторизации), эти три сервиса построены **по одному и тому же шаблону**, используют одни и те же паттерны и технологии. Они отличаются только своей предметной областью (профили / события / клубы), а каркас идентичен. Ниже — общий чертёж на примере `ClubService`; `UserService` и `EventService` устроены аналогично.

### CQRS на MediatR на примере ClubService

```
ClubManagement/
  Command/CreateClubCommand/
    CreateClubCommand.cs           # команда (запрос на изменение)
    CreateClubCommandHandler.cs    # обработчик
    CreateClubCommandValidator.cs  # FluentValidation-валидатор
  Queries/GetClubQuery/
    GetClubQuery.cs                # запрос на чтение
    GetClubQueryHandler.cs
    GetClubQueryValidator.cs
  Common/                          # *Result-DTO результатов
    CreateClubResult.cs
    GetClubResult.cs
```

- Контроллер **тонкий**: принимает `*Request` из `Contracts`, маппит его в команду/запрос через **Mapster** и отправляет через MediatR `ISender`.
- Валидация выполняется автоматически в конвейере через `ValidationBehavior` (`IPipelineBehavior`), который подхватывает соответствующий `AbstractValidator`.

### Domain-Driven Design

- **`AggregateRoot<TId>`** наследует `Entity<TId>` и реализует `IEventSourceable` — накапливает доменные события (`AddDomainEvent` / `ClearDomainEvents`).
- **Доменные события** (`IDomainEvent`): например, `ClubCreatedDomainEvent`, `ClubDeletedDomainEvent`.
- **Value Objects** и **строго типизированные идентификаторы** (`ClubId`, `OwnerId`, `UserId`) реализуют `IEntityId` и наследуют `ValueObject`.
- **`Enumeration`** — типобезопасные перечисления вместо `enum`.
- Создание сущностей — через фабричные методы `Create(...)`.

### Repository + Specification

Доступ к данным абстрагирован, бизнес-логика не зависит от EF Core напрямую:

- **`IRepository` / `IReadRepository`** + реализация `GenericRepository<T>`.
- **`IUnitOfWork`** — управление транзакциями и `SaveChanges`.
- **Specification pattern**: `ISpecification` → `BaseSpecification` → конкретные спецификации (`ClubByIdSpec`, `SearchClubsSpec` и т.д.), применяемые через `SpecificationEvaluator`. Это выносит критерии запросов (фильтры, include, пагинацию, сортировку) из хендлеров в переиспользуемые объекты.

### Transactional Outbox + Kafka

Надёжная публикация интеграционных событий реализована через паттерн **Transactional Outbox** — событие сохраняется в БД в одной транзакции с бизнес-данными, а отправляется в Kafka асинхронно:

1. `UnitOfWork` при сохранении превращает доменные события в **интеграционные** (`IIntegrationEvent`, маппинг через Mapster) и пишет их в таблицу `OutboxMessage`.
2. **`OutboxProcessorBackgroundService`** (фоновый `BackgroundService`) опрашивает таблицу (`FOR UPDATE SKIP LOCKED`) и публикует события через `KafkaIntegrationEventDispatcher` / `IKafkaProducerFactory`.
3. Сообщения, которые не удалось обработать, отправляются в **dead-letter-очередь** (`DeadLetterMessage`); тип события определяется через `IEventTypeMapper`.

### Сквозная обработка ошибок

- Глобальные обработчики `GlobalExceptionHandler` и `ValidationExceptionHandler` (реализуют `IExceptionHandler`) превращают исключения в корректные HTTP-ответы.
- Доменные/прикладные исключения: `EntityNotFoundException`, `EntityAlreadyExistsException`, `AlreadyDoneException`.

---

## 🔗 Межсервисное взаимодействие

- **Синхронно (gRPC):** `AuthService.PermissionOracle` отдаёт роли и разрешения другим сервисам для авторизации.
- **Синхронно (REST):** внешние клиенты обращаются к публичным HTTP-эндпоинтам сервисов.
- **Асинхронно (Kafka):** доменные события (`auth`, `userprofile`, `club`) публикуются и потребляются заинтересованными сервисами. Bootstrap-серверы настраиваются через `KAFKAOPTIONS__<scope>__BOOTSTRAPSERVERS`.

---

## 📂 Структура решения

```
WingDing-Party/
├── WingDing-Party.sln           # Корневой solution (все сервисы)
├── docker-compose.yml           # Прикладные сервисы
├── docker-compose.override.yml  # Инфраструктура (Keycloak, Postgres, Redis, Kafka, MinIO, pgAdmin)
├── Directory.Packages.props     # Централизованное управление версиями пакетов
├── .files/                      # Импортируемый realm Keycloak
└── src/
    ├── AuthService/
    │   ├── AuthService.Api
    │   ├── AuthService.Application
    │   ├── AuthService.Domain
    │   ├── AuthService.Infrastructure
    │   └── AuthService.Contracts
    ├── UserService/   (аналогичная структура)
    ├── EventService/  (аналогичная структура)
    └── ClubService/   (аналогичная структура)
```

---

## 🚀 Как запустить

### Предварительные требования

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) и Docker Compose

### 1. Поднять инфраструктуру и сервисы

```bash
# Весь стек (инфраструктура + сервисы)
docker compose up -d

# Только инфраструктура для локальной разработки
docker compose up -d postgres redis keycloak kafka minio
```

> Файл `.env.development` содержит значения по умолчанию для портов, учётных данных и строк подключения. Realm Keycloak `wingding-party` импортируется при старте из `.files/`.

### 2. Запустить сервис локально

```bash
dotnet run --project src/AuthService/AuthService.Api
```

### 3. Сборка и миграции

```bash
# Собрать весь solution
dotnet build WingDing-Party.sln

# Создать миграцию
dotnet ef migrations add <MigrationName> \
  --project src/<Service>/<Service>.Infrastructure \
  --startup-project src/<Service>/<Service>.Api
```

> Миграции применяются **автоматически** при старте сервиса через `app.ApplyMigrations()` в `Program.cs`.

### 4. Документация API

После запуска Swagger доступен по адресу `http://localhost:<порт>/swagger`.

---

## 🌐 Порты и инфраструктура

| Компонент | Порт |
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
| MinIO (API / Console) | 9000 / 9001 |
| pgAdmin | 5050 |

---

## 🗺 Дорожная карта

Планируемые к реализации сервисы и возможности:

- 🚧 **Chat Service** — real-time общение (WebSocket / gRPC-Web), история сообщений.
- 🚧 **Notification Service** — уведомления и рассылки (Worker Service).
- 🚧 **Search Service** — полнотекстовый поиск (Elasticsearch) по событиям, пользователям и клубам.
- 🚧 **Recommendation** — рекомендации событий на основе интересов пользователя.
- 🚧 **API Gateway** — единая точка входа и маршрутизация.
- 🚧 **Ticket Service** — интеграция с билетными системами для платных событий.

---

## 📄 Лицензия

Проект распространяется на условиях, указанных в файле [LICENSE](LICENSE).
