# AuthService

Сервис аутентификации и авторизации. Управляет регистрацией пользователей через Keycloak, выдачей JWT токенов, ролями и разрешениями.

## Knowledge Graph

Граф знаний для AuthService находится в `.claude/graphify-out/` в корне репозитория.

**Перед тем как читать файлы исходного кода**, проверь наличие графа:

```powershell
Test-Path ../../.claude/graphify-out/graph.json
```

Если граф существует — используй его для навигации по архитектуре, поиска зависимостей и понимания структуры. Это экономит контекст: вместо чтения десятков файлов достаточно запросить нужный узел или путь в графе. Обращайся к исходникам только для деталей реализации, которых нет в графе.

Пересобрать или обновить граф: `/graphify src/AuthService` (incremental: `/graphify src/AuthService --update`).

## Авторизация (permission-based)

Система разрешений поверх стандартного ASP.NET Core Authorization:

1. **`[HasPermission("events:create")]`** — атрибут на endpoint (создаёт динамическую policy)
2. **`PermissionAuthorizationPolicyProvider`** — превращает имя permission в policy
3. **`PermissionAuthorizationHandler`** — проверяет claim `permission` у пользователя
4. **`CustomClaimsTransformation`** — после валидации JWT, обогащает ClaimsPrincipal данными из нашей БД: заменяет Keycloak `sub` на внутренний `User.Id`, добавляет роли из PostgreSQL
5. **`AuthorizationService`** — запрашивает роли/permissions из БД, кеширует в Redis (TTL 5 мин, ключ `auth:roles-{identityId}`)

Разрешения определены как static readonly поля в `Permission : Enumeration` (например, `Permission.EventsCreate`).

## Интеграция с Keycloak

- **`IAuthenticationService`** / `AuthenticationService` — регистрация пользователя через Keycloak Admin API (`/admin/realms/wingding-party/users`)
- **`IJwtService`** / `JwtService` — получение JWT токена через ROPG Flow (`/realms/wingding-party/protocol/openid-connect/token`)
- **`AdminAuthorizationDelegatingHandler`** — автоматически добавляет Bearer-токен admin-клиента к исходящим запросам к Admin API
- JWT Bearer настраивается через `JwtBearerOptionsSetup` (читает `Authentication` секцию из конфига)

## Конфигурация

Конфиг-классы биндятся на root configuration (не на секцию), то есть переменные окружения (`CONNECTION_STRING`, `REDIS_CONNECTION_STRING`) маппируются напрямую. Секции `Authentication` и `Keycloak` биндятся явно через `configuration.GetSection(...)`.
