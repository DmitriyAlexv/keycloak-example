# KeyCloakExample

Минимальное ASP.NET Core приложение для демонстрации аутентификации и авторизации через [Keycloak](https://www.keycloak.org/) по протоколу OpenID Connect (Authorization Code Flow + PKCE).

## Структура

```
KeyCloakExample.sln
├── src/KeyCloakExample.Web/     # ASP.NET Core приложение (OIDC-клиент)
├── keycloak/realm-import.json   # Realm "demo": клиент, роль, пользователь
└── docker-compose.yml           # Keycloak в режиме start-dev с импортом realm
```

## Как это работает

- Приложение построено на Razor Pages (страницы в `src/KeyCloakExample.Web/Pages`) и использует `Microsoft.AspNetCore.Authentication.OpenIdConnect`.
- Схемы: `Cookies` (сессия приложения) + `OpenIdConnect` (challenge/logout в Keycloak).
- В Keycloak встроенный клиентский scope `roles` переопределён ([`keycloak/realm-import.json`](keycloak/realm-import.json)): мапперы `realm roles` и `client roles` кладут роли в multivalued-клейм `roles` (массив строк) в ID- и access-токенах.
- В приложении `TokenValidationParameters.RoleClaimType = "roles"`, поэтому `[Authorize(Roles = "demo-user")]` работает «из коробки» (в примере — страница `/admin`), без ручного разбора `realm_access`.
- Access token сохраняется (`SaveTokens = true`) и выводится на странице `/secure`.

## Quick Start

### 1. Поднять Keycloak

```bash
docker compose up -d
```

Keycloak поднимется на <http://localhost:8080>. При старте автоматически импортируется realm `demo` из [`keycloak/realm-import.json`](keycloak/realm-import.json):

| Сущность | Значение |
|---|---|
| Realm | `demo` |
| Консоль администратора | <http://localhost:8080/admin> — `admin` / `admin` |
| OIDC-клиент | `keycloak-example-web` (secret: `change-me-in-production`) |
| Redirect URI | `http://localhost:5165/*` |
| Realm-роль | `demo-user` |
| Демо-пользователь | `alice` / `password` (с ролью `demo-user`) |

Подождите ~10–30 секунд, пока Keycloak выполнит импорт realm (следите за строкой `Imported realm "demo"` в логах: `docker compose logs keycloak`).

### 2. Запустить приложение

```bash
dotnet run --project src/KeyCloakExample.Web --launch-profile http
```

Приложение поднимется на <http://localhost:5165>.

### 3. Проверить

1. Откройте <http://localhost:5165> и перейдите по ссылке **«Войти через Keycloak»** (`/secure`).
2. Вы будете перенаправлены на страницу логина Keycloak — войдите как `alice` / `password`.
3. После входа вы вернётесь в приложение и увидите свои клеймы и access token на странице `/secure`.
4. Откройте `/admin` — страница доступна только пользователям с realm-ролью `demo-user` (у `alice` она есть).
5. `/logout` завершает сессию и в приложении, и в Keycloak.

## Конфигурация

Настройки Keycloak находятся в [`src/KeyCloakExample.Web/appsettings.json`](src/KeyCloakExample.Web/appsettings.json) и могут быть переопределены переменными окружения:

| Ключ | Переменная окружения | Значение по умолчанию |
|---|---|---|
| `Keycloak:Authority` | `Keycloak__Authority` | `http://localhost:8080/realms/demo` |
| `Keycloak:ClientId` | `Keycloak__ClientId` | `keycloak-example-web` |
| `Keycloak:ClientSecret` | `Keycloak__ClientSecret` | `change-me-in-production` |
| `Keycloak:RequireHttpsMetadata` | `Keycloak__RequireHttpsMetadata` | `false` (для dev по http) |