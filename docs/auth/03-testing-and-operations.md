# 03 — Как тестировать и работать с AuthService

> Практический документ. Здесь — как поднять инфраструктуру, получить токен, разобрать его,
> запустить сервис, дёрнуть защищённый эндпоинт, заглянуть в БД/Redis/логи. Все команды — под
> PowerShell (Windows). Концепции — в [00](00-auth-concepts.md), реализация — в
> [01](01-authservice-flows.md)/[02](02-class-reference.md).
>
> ⚠️ В разделе [4](#4-несостыковки-конфигурации-которые-ты-встретишь) собраны реальные
> расхождения между `appsettings`, `docker-compose` и realm-экспортом. **Прочитай его до
> первого запуска** — иначе упрёшься в «не подключается» и «401».

## Содержание
1. [Что сейчас реально работает, а что заглушка](#1-что-сейчас-реально-работает)
2. [Поднять инфраструктуру](#2-поднять-инфраструктуру)
3. [Проверить, что Keycloak импортировал realm](#3-проверить-keycloak)
4. [⚠️ Несостыковки конфигурации](#4-несостыковки-конфигурации-которые-ты-встретишь)
5. [Создать тестового пользователя](#5-создать-тестового-пользователя)
6. [Получить токен](#6-получить-токен)
7. [Разобрать токен](#7-разобрать-токен)
8. [Запустить AuthService](#8-запустить-authservice)
9. [Подготовить пользователя в authdb](#9-подготовить-пользователя-в-authdb)
10. [Дёрнуть защищённый эндпоинт](#10-дёрнуть-защищённый-эндпоинт)
11. [Заглянуть в БД, Redis, логи](#11-заглянуть-в-бд-redis-логи)
12. [Траблшутинг: 401 vs 403 и прочее](#12-траблшутинг)

---

## 1. Что сейчас реально работает

Прежде чем тестировать — пойми, что эндпоинты `register`/`login` сейчас **заглушки**
(`UsersController.cs`, документ 01). Поэтому «честный» сквозной тест через REST невозможен,
пока их не дописали (документ 04). Но проверить можно **почти всё остальное**:

| Что | Состояние | Как проверить |
|---|---|---|
| Keycloak realm + клиенты | ✅ импортируется из `.files/` | раздел 3 |
| Получение токена (ROPC) | ✅ напрямую через Keycloak | раздел 6 |
| Загрузка AuthService, миграции | ✅ авто на старте | раздел 8 |
| Валидация JWT (`UseAuthentication`) | ✅ рабочий pipeline | раздел 10 |
| `[HasPermission]` (`UseAuthorization`) | ✅ рабочий pipeline | раздел 10 |
| gRPC `PermissionOracle` | ✅ смонтирован | раздел 10 |
| Redis-кэш прав | ✅ | раздел 11 |
| `POST /register` | 🚧 заглушка | — (документ 04) |
| `POST /login` | 🚧 заглушка | можно эмулировать через раздел 6 |

Главное следствие: чтобы протестировать **авторизацию** (`/api/users/me`), нужны две вещи
одновременно: (а) валидный токен из Keycloak и (б) запись `User` в `authdb` с тем же
`identityId`. Поскольку регистрация — заглушка, запись в `authdb` пока заводим вручную
(раздел 9).

---

## 2. Поднять инфраструктуру

```powershell
# Из корня репозитория. Поднимет всё (Keycloak, Postgres, Redis, pgAdmin, Kafka, MinIO).
docker compose up -d

# Минимально для AuthService достаточно:
docker compose up -d keycloak postgres redis pgadmin
```

> `docker-compose.override.yml` подхватывается автоматически — инфраструктура (Keycloak,
> Postgres, Redis, pgAdmin, Kafka, MinIO) описана именно там, а прикладные сервисы — в
> `docker-compose.yml`.

Порты и UI (значения по умолчанию, переопределяются переменными окружения):

| Сервис | URL / порт | Доступ |
|---|---|---|
| Keycloak (admin console) | http://localhost:18080 | `admin` / `admin` |
| AuthService (в docker) | http://localhost:5200 | — |
| AuthService (локально, `dotnet run`) | http://localhost:5228 | см. раздел 8 ⚠️ |
| PostgreSQL | localhost:5432 | `postgres` / `postgres` |
| Redis | localhost:6379 | пароль `redis_password` ⚠️ |
| pgAdmin | http://localhost:5050 | `admin@admin.com` / `admin` |
| Kafka UI | http://localhost:8090 | `admin` / `admin` |

Проверить, что контейнеры живы и healthy:

```powershell
docker compose ps
docker compose logs -f keycloak   # дождись "Running the server in development mode"
```

Имена контейнеров (пригодятся для `docker exec`): `wingding_identity_provider` (Keycloak),
`wingding_postgres`, `wingding_redis`, `wingding_pgadmin`.

---

## 3. Проверить Keycloak

Realm импортируется при старте из `.files/wingding-realm-export.json` (флаг
`--import-realm` в `docker-compose.override.yml`). В нём заведены **два клиента** (документ 01,
раздел 2):

| clientId | Тип | Что включено |
|---|---|---|
| `wingding-public-client` | public | `directAccessGrants` (ROPC) ✅, `standardFlow` + PKCE S256 ✅, redirect на localhost:3000/5000 |
| `wingding-admin-client` | confidential | `serviceAccounts` (client_credentials) ✅, secret `CHANGE_ME_IN_PRODUCTION` |

Проверь, что realm поднялся:

```powershell
# Discovery document — если открывается, realm жив и JWKS доступен
start http://localhost:18080/realms/wingding-party/.well-known/openid-configuration
```

В админке (http://localhost:18080 → realm `wingding-party` → Clients) убедись, что оба
клиента на месте.

> 📌 **Realm-экспорт минимален**: в нём нет пользователей и **нет ролевых маппингов для
> service-account админ-клиента**. Это важно для раздела 5 (создание пользователей через
> Admin API).

---

## 4. Несостыковки конфигурации, которые ты встретишь

Это раздел-предупреждение. Сейчас в проекте три источника настроек Keycloak/инфры, и они
**не согласованы между собой**. Не баг в логике auth — но запуск «из коробки» спотыкается.
Перечисляю, чтобы ты понимал, что чинить (и заодно — отличная практическая задача, чтобы
разобраться).

| # | Где | Что написано | Проблема | Как привести в порядок |
|---|---|---|---|---|
| 1 | `docker-compose.yml` (auth-service env) | `KEYCLOAK__BASEURL=http://keycloak:18080` | Внутри docker-сети Keycloak слушает **8080**, а `18080` — это только проброс на хост (`18080:8080`). Из контейнера `keycloak:18080` недоступен. | В env заменить внутренние URL на порт **8080**: `http://keycloak:8080/...` |
| 2 | `appsettings.Development.json` | хосты `wingding-idp:8080`, `wingding-redis:6379` | Таких сетевых имён нет: сервисы называются `keycloak` и `redis` (контейнеры — `wingding_identity_provider`, `wingding_redis`), сетевых alias'ов не задано. | Либо добавить `aliases` в `networks` сервисов, либо использовать реальные имена (`keycloak`, `redis`) |
| 3 | `appsettings.Development.json` (Cache) | `"Cache": "wingding-redis:6379"` без пароля | Redis поднят **с паролем** (`--requirepass redis_password`). Подключение без пароля упадёт. | Добавить пароль в строку подключения: `...,password=redis_password` |
| 4 | `launchSettings.json` vs CLAUDE.md/compose | локально — **5228**, в docker и доке — **5200** | При `dotnet run` сервис слушает 5228/7041, а не 5200. | Помни про это при локальных запросах, либо поправь `applicationUrl` |
| 5 | realm-экспорт | у `wingding-admin-client` нет роли `realm-management: manage-users` | service-account не сможет создавать пользователей через Admin API → 403. | Назначить роль (раздел 5) или дописать в realm-экспорт |
| 6 | `Api/DependencyInjection.cs:49-60` (`BindConfigurations`) | биндятся только `AuthDatabaseOptions`, `AuthenticationOptions`, `KeycloakOptions` | **`RedisOptions` не зарегистрирован** → `IOptions<RedisOptions>` отдаёт пустой объект, и строка подключения к Redis собирается некорректно (`:6379` без хоста/пароля). | Добавить `services.Configure<RedisOptions>(configuration.Bind);` в `BindConfigurations` |

> 🔑 **Как на самом деле биндятся настройки.** Конфиг-классы биндятся на **root**
> configuration (`configuration.Bind`), то есть ключи — это **верхнеуровневые** имена свойств,
> а не секции. Реальные ключи (сверено с классами `AuthDatabaseOptions`/`RedisOptions`):
> - БД: `CONNECTION_STRING` (свойство `AuthDatabaseOptions.CONNECTION_STRING`).
> - Redis: `REDIS_HOST`, `REDIS_PORT`, `REDIS_PASSWORD` — строка подключения **вычисляется**
>   из них (`RedisOptions.REDIS_CONNECTION_STRING`, который сам добавляет `,password=...`).
>
> Именно поэтому `docker-compose.yml` задаёт `CONNECTION_STRING=...` и `REDIS_HOST/PORT/PASSWORD`
> как переменные окружения, а ключи `ConnectionStrings:Database`/`Cache` из
> `appsettings.Development.json` фактически **не читаются** этими опциями.

### Известно-рабочая локальная конфигурация

Если запускаешь AuthService **локально** (`dotnet run`), все docker-внутренние имена нужно
заменить на `localhost`. Не правь `appsettings` (он общий) — задай переопределения через
**User Secrets** (они вне git и перекрывают appsettings, порядок источников — см. CLAUDE.md).
Ключи — root-уровня, как разобрано выше:

```powershell
cd src/AuthService/AuthService.Api
dotnet user-secrets init

# --- БД (root-ключ CONNECTION_STRING) ---
dotnet user-secrets set "CONNECTION_STRING" "Host=localhost;Port=5432;Database=authdb;Username=postgres;Password=postgres"

# --- Redis (root-ключи; строка подключения соберётся сама, с паролем) ---
# ВНИМАНИЕ: сработает только после исправления несостыковки №6 (регистрация RedisOptions)
dotnet user-secrets set "REDIS_HOST" "localhost"
dotnet user-secrets set "REDIS_PORT" "6379"
dotnet user-secrets set "REDIS_PASSWORD" "redis_password"

# --- Keycloak / валидация JWT (эти биндятся через GetSection, секционные ключи) ---
dotnet user-secrets set "Authentication:Issuer" "http://localhost:18080/realms/wingding-party"
dotnet user-secrets set "Authentication:MetadataUrl" "http://localhost:18080/realms/wingding-party/.well-known/openid-configuration"
dotnet user-secrets set "Authentication:Audience" "account"
dotnet user-secrets set "Authentication:RequireHttpsMetadata" "false"
dotnet user-secrets set "Keycloak:BaseUrl" "http://localhost:18080"
dotnet user-secrets set "Keycloak:AdminUrl" "http://localhost:18080/admin/realms/wingding-party/"
dotnet user-secrets set "Keycloak:TokenUrl" "http://localhost:18080/realms/wingding-party/protocol/openid-connect/token"
dotnet user-secrets set "Keycloak:AdminClientId" "wingding-admin-client"
dotnet user-secrets set "Keycloak:AdminClientSecret" "CHANGE_ME_IN_PRODUCTION"
dotnet user-secrets set "Keycloak:AuthClientId" "wingding-public-client"
```

> Тонкость: `AuthDatabaseOptions`/`KeycloakOptions` биндятся на **root**
> (`Api/DependencyInjection.cs:53-55`), а `Authentication`/`Keycloak` ещё раз — через
> `GetSection(...)` в `Infrastructure/DependencyInjection.cs:52-55`. Поэтому для
> `Authentication`/`Keycloak` рабочие ключи — **секционные** (`Authentication:Issuer`), а для
> строки БД — **root** (`CONNECTION_STRING`). Если что-то не подхватилось — сверься с этими
> двумя местами регистрации.

---

## 5. Создать тестового пользователя

Регистрация в нашем API — заглушка, поэтому пользователя для тестов заводим прямо в Keycloak.
Два способа.

### Способ А — через админ-консоль (просто)
1. http://localhost:18080 → войти `admin`/`admin`.
2. Realm `wingding-party` → **Users** → **Add user**: Username/Email = `test@wingding.local`,
   Email verified = On → Create.
3. Вкладка **Credentials** → **Set password**: задай пароль, Temporary = **Off**.
4. Скопируй **ID** пользователя со вкладки Details — это `identityId` (тот самый `sub`).
   Он понадобится в разделе 9.

### Способ Б — через Admin API (как это делает AuthService)

Это ровно то, что делает `AuthenticationService` + `AdminAuthorizationDelegatingHandler`
(документ 01, раздел 3). Полезно повторить руками, чтобы прочувствовать client_credentials.

```powershell
# 1) Получить admin-токен (client_credentials, как AdminAuthorizationDelegatingHandler)
$admin = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:18080/realms/wingding-party/protocol/openid-connect/token" `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{ client_id="wingding-admin-client"; client_secret="CHANGE_ME_IN_PRODUCTION";
           grant_type="client_credentials"; scope="openid" }
$adminToken = $admin.access_token

# 2) Создать пользователя (как AuthenticationService.RegisterAsync)
Invoke-WebRequest -Method Post `
  -Uri "http://localhost:18080/admin/realms/wingding-party/users" `
  -Headers @{ Authorization = "Bearer $adminToken" } `
  -ContentType "application/json" `
  -Body (@{
     firstName="Test"; lastName="User"; email="test@wingding.local";
     username="test@wingding.local"; enabled=$true; emailVerified=$true;
     credentials=@(@{ type="password"; value="Passw0rd!"; temporary=$false })
  } | ConvertTo-Json -Depth 5)
# В ответе смотри заголовок Location: .../users/{identityId} — это ExtractIdentityIdFromLocationHeader
```

> 🛑 **Если шаг 2 вернул `403 Forbidden`** — это несостыковка №5 из раздела 4: у
> service-account админ-клиента нет права создавать пользователей. Исправь: админка → Clients
> → `wingding-admin-client` → **Service accounts roles** → Assign role → отфильтруй по
> `realm-management` → выдай **`manage-users`** (для полноты — ещё `view-users`). Повтори шаг 2.

---

## 6. Получить токен

Эмулируем `JwtService` (ROPC через public-client). `wingding-public-client` —
`publicClient: true`, поэтому **секрет не нужен**, а `directAccessGrantsEnabled: true`
разрешает password-grant.

```powershell
$resp = Invoke-RestMethod -Method Post `
  -Uri "http://localhost:18080/realms/wingding-party/protocol/openid-connect/token" `
  -ContentType "application/x-www-form-urlencoded" `
  -Body @{
     client_id = "wingding-public-client"
     grant_type = "password"
     username = "test@wingding.local"
     password = "Passw0rd!"
     scope = "openid email"
  }

$token = $resp.access_token
$token   # вывести JWT
$resp | Format-List   # увидеть access_token, id_token, refresh_token, expires_in
```

Обрати внимание: Keycloak вернул и `id_token`, и `refresh_token` — но `JwtService` в коде
читает только `access_token` (документ 01, раздел 4). Здесь ты видишь все три.

---

## 7. Разобрать токен

JWT не зашифрован — его можно прочитать (документ 00, раздел 5). Способы:

- **Онлайн:** вставь `$token` на https://jwt.io/ — увидишь header, payload (claims), и сверку
  подписи (если подставить JWKS realm).
- **Локально в PowerShell** — раскодировать payload без проверки подписи:

```powershell
$payload = $token.Split('.')[1]
# дополнить Base64URL до кратности 4 и декодировать
$payload = $payload.Replace('-','+').Replace('_','/')
switch ($payload.Length % 4) { 2 { $payload += '==' } 3 { $payload += '=' } }
[Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($payload)) | ConvertFrom-Json | Format-List
```

На что смотреть в payload (всё это проверяет `JwtBearerOptionsSetup`):

| Claim | Что значит | С чем сверяется |
|---|---|---|
| `iss` | издатель | `Authentication:Issuer` |
| `aud` | аудитория | `Authentication:Audience` (`account`) — частая причина 401 |
| `sub` | id пользователя в Keycloak = **identityId** | ключ поиска в `authdb` |
| `exp` / `iat` | срок жизни | проверка протухания |
| `email`, `preferred_username` | профиль | — |

> 🔎 Запомни `sub` из этого токена — это `identityId`, который нужен в разделе 9.

---

## 8. Запустить AuthService

### Вариант А — в docker (рекомендуется для сквозного теста)
Сервис в одной сети с Keycloak/Postgres/Redis, имена резолвятся. Но сначала примени
исправления №1–3 из раздела 4 (иначе `keycloak:18080` и пароль Redis сломают старт).

```powershell
docker compose up -d --build auth-service
docker compose logs -f auth-service   # дождись применения миграций и старта Kestrel
# Swagger:
start http://localhost:5200/swagger
```

### Вариант Б — локально (быстрее для отладки кода)
Требует User Secrets из раздела 4 (всё на `localhost`). Инфраструктура при этом в docker.

```powershell
docker compose up -d keycloak postgres redis
dotnet run --project src/AuthService/AuthService.Api
# Слушает http://localhost:5228 (профиль http), НЕ 5200 — см. несостыковку №4
start http://localhost:5228/swagger
```

При старте сервис автоматически применит миграции (`ApplyMigrations()` в `Program.cs:27`) и
засидит роли/разрешения. Проверить — в разделе 11.

> Дальше в примерах беру локальный порт **5228**. Для docker-запуска меняй на **5200**.

---

## 9. Подготовить пользователя в authdb

Чтобы `[HasPermission]` прошёл, в `authdb` должна быть запись `User` с `identity_id`, равным
`sub` твоего токена. Так как регистрация — заглушка, вставим вручную. Возьми `identityId` из
раздела 5/7.

```powershell
# Зайти в psql внутри контейнера Postgres
docker exec -it wingding_postgres psql -U postgres -d authdb
```

```sql
-- В psql. Подставь свой identityId вместо <IDENTITY_ID>.
-- 1) создать пользователя
INSERT INTO users (id, first_name, last_name, email, identity_id)
VALUES (gen_random_uuid(), 'Test', 'User', 'test@wingding.local', '<IDENTITY_ID>')
RETURNING id;

-- 2) выдать роль Registered (id=2; роли засижены миграцией, см. RoleConfiguration)
INSERT INTO user_roles (roles_id, users_id)
VALUES (2, '<USER_ID_ИЗ_ШАГА_1>');
```

> Имена таблиц — snake_case (`UseSnakeCaseNamingConvention`, документ 02). Точные имена
> столбцов join-таблицы `user_roles` проверь в pgAdmin, если INSERT ругается (EF генерит их
> по соглашению, и они могут отличаться). Роль `Registered` даёт права `events:read`,
> `clubs:read`, `users:read` (документ 01, раздел 9) — значит `users:read` для `/me` будет.

---

## 10. Дёрнуть защищённый эндпоинт

Теперь есть всё: токен (`$token`) + пользователь в `authdb` с тем же `identityId`.

```powershell
# /api/users/me защищён [HasPermission(Permissions.UsersRead)]
Invoke-WebRequest -Method Get -Uri "http://localhost:5228/api/users/me" `
  -Headers @{ Authorization = "Bearer $token" } | Select-Object StatusCode, Content
```

Что произойдёт под капотом (документ 01, раздел 5):
1. `UseAuthentication` проверит подпись/issuer/audience/exp токена.
2. `RemoteClaimsTransformation` по `sub`(=identityId) сходит в `authdb` (через
   `LocalPermissionService` → `AuthorizationService`), подтянет роли и внутренний `User.Id`.
3. `PermissionAuthorizationHandler` проверит наличие `users:read`. Есть → **200 OK**.

Сравни поведение:
- **Без заголовка** `Authorization` → **401 Unauthorized** (не прошёл AuthN).
- **С токеном, но без записи в `authdb`** → `RemoteClaimsTransformation` упадёт (нет
  пользователя) или прав не будет → **403 / ошибка**.
- **С токеном и записью, но роль без нужного права** → **403 Forbidden** (прошёл AuthN, не
  прошёл AuthZ).

### Проверить gRPC `PermissionOracle`
Это межсервисный канал (документ 01, раздел 6). Дёрнуть можно через `grpcurl` (если стоит):

```powershell
# Список методов (reflection может быть выключен — тогда укажи .proto явно)
grpcurl -plaintext localhost:5200 list
# Вызов GetPermissions
grpcurl -plaintext -d '{ \"identity_id\": \"<IDENTITY_ID>\" }' `
  localhost:5200 authservice.PermissionOracle/GetPermissions
```

> gRPC слушает на том же Kestrel (HTTP/2 включён через `Kestrel:Protocols=Http1AndHttp2`).
> В реальной интеграции его зовёт `GrpcPermissionService` из других сервисов, не фронтенд.

---

## 11. Заглянуть в БД, Redis, логи

### PostgreSQL (через pgAdmin или psql)
- pgAdmin: http://localhost:5050 → подключись к серверу `wingding_postgres` (host `postgres`
  или `wingding_postgres`, port 5432, postgres/postgres) → база `authdb`.
- Полезные таблицы: `users`, `roles`, `permissions`, `role_permissions`, `user_roles`,
  `__ef_migrations_history` (проверить, что миграция применилась).

```sql
SELECT * FROM users;
SELECT r.role_type, p.name FROM role_permissions rp
  JOIN roles r ON r.id = rp.role_id
  JOIN permissions p ON p.id = rp.permission_id
  ORDER BY r.id;   -- увидеть матрицу прав из сидинга
```

### Redis (кэш ролей/прав)
После первого запроса к `/me` в Redis появятся ключи кэша (документ 01, раздел 5):

```powershell
docker exec -it wingding_redis redis-cli -a redis_password
```
```
KEYS auth:*
GET auth:permissions-<IDENTITY_ID>
TTL auth:roles-<IDENTITY_ID>     # увидишь ~300 сек (5 мин)
```

> Чтобы проверить «мгновенную» актуализацию прав: поменяй роль пользователя в `authdb`,
> сделай `DEL auth:permissions-<id> auth:roles-<id>` в Redis — на следующем запросе права
> пересоберутся из БД.

### Логи (Serilog)
Раздельные файловые синки настроены в `Api/DependencyInjection.cs:81-123`:

```powershell
# Локальный запуск пишет относительно рабочего каталога процесса (../logs/...)
Get-Content src/AuthService/logs/Information/log-*.txt -Tail 50 -Wait
# В docker логи примонтированы:
docker compose logs -f auth-service        # консоль
#   + том ./logs/auth-service (см. volumes в docker-compose.yml)
```

Уровни разведены по папкам: `Information/`, `Warning/`, `Error/`.

---

## 12. Траблшутинг

| Симптом | Вероятная причина | Что делать |
|---|---|---|
| **401** на `/me` без явной причины | `aud` токена ≠ `Authentication:Audience` (`account`) | посмотри `aud` в токене (раздел 7); при необходимости поправь `Audience` или добавь audience-mapper в Keycloak |
| **401**, в логах «unable to obtain configuration from metadata» | сервис не достучался до `MetadataUrl` (JWKS) | проверь хост/порт Keycloak (несостыковки №1/№2), `RequireHttpsMetadata=false` |
| **403** на `/me` | AuthN прошла, нет права `users:read` | проверь, что у пользователя в `authdb` есть роль с этим правом (раздел 9) |
| Ошибка в `RemoteClaimsTransformation` / `FirstAsync` | нет записи `User` в `authdb` с этим `identityId` | заведи пользователя (раздел 9) |
| **403** при создании пользователя через Admin API | service-account без `manage-users` | назначь роль (раздел 5, способ Б) |
| Старт падает на подключении к Redis | `RedisOptions` не зарегистрирован / пароль не задан | несостыковки №6 и №3 |
| Старт падает на БД (`Npgsql`, host) | docker-имена не резолвятся локально | User Secrets на `localhost` (раздел 4) |
| `column ... does not exist` при миграции | рассинхрон схемы/снапшота | сбрось том БД: `docker compose down -v` затем `up` (внимание: удалит данные) |
| Запрос на 5200 локально не отвечает | локально порт **5228** | используй 5228 или поправь `launchSettings` (несостыковка №4) |

### Быстрый чек-лист «ничего не работает»
```powershell
docker compose ps                                   # все healthy?
start http://localhost:18080/realms/wingding-party/.well-known/openid-configuration  # realm жив?
docker compose logs --tail=50 auth-service          # что в логах сервиса?
```

---

➡️ Дальше: [04 — Дорожная карта](04-roadmap.md) — что доделать (register/login, refresh,
PKCE), как поднять API Gateway (YARP) и распространить auth на остальные сервисы.
