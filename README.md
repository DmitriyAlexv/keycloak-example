# KeyCloakExample

- ASP.NET 10
- KeyCloak 26.8
- OIDC

## Main Endpoints

- http://localhost:5165/ - UI приложения
- http://localhost:8080/admin/master/console/ - UI настройки keycloak
- http://localhost:8080/realms/demo/account/ - UI профиля области demo keycloak

## Quick Start

### 1. Поднять Keycloak

```bash
docker compose up -d
```

При старте автоматически импортируется realm `demo` из [`keycloak/realm-import.json`](keycloak/realm-import.json)

Подождите ~10–30 секунд, пока Keycloak выполнит импорт realm.

### 2. Запустить приложение

```bash
dotnet run --project src/KeyCloakExample.Web --launch-profile http
```

Конфигурация в [`src/KeyCloakExample.Web/appsettings.json`](src/KeyCloakExample.Web/appsettings.json)

### 3. Проверить

1. Откройте <http://localhost:5165> и перейдите по ссылке **«Войти через Keycloak»** (`/secure`).
2. Вы будете перенаправлены на страницу логина Keycloak — войдите как `alice` / `password`.
3. После входа вы вернётесь в приложение и увидите свои клеймы и access token на странице `/secure`.
4. Откройте `/admin` — страница доступна только пользователям с realm-ролью `demo-user` (у `alice` она есть).
5. `/logout` завершает сессию и в приложении, и в Keycloak.