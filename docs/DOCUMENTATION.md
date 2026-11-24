# Authentication

## Overview

The application supports optional authentication based on user accounts stored in the database.

Authentication is automatically enabled when at least one active user has a password configured. If no active user has a password, the application starts without requiring a login.

## Startup Flow

1. Load application settings.
2. Apply theme and localization.
3. Execute database migrations.
4. Determine whether authentication is required.
5. Store the result in `ApplicationState.IsAuthenticationRequired`.

## Authentication Requirement

Authentication is considered required when at least one active user has a non-empty password.

```csharp
await IUserRepository.IsAuthenticationRequiredAsync();
```

## Login Process

User authentication is performed through `IUserRepository`.

```csharp
await IUserRepository.LoginAsync(
    userName,
    password);
```

A login succeeds only when:

- The user exists.
- The user account is active.
- The provided password matches the stored encrypted password.

After successful authentication:

- `ApplicationState.CurrentUser` is assigned.
- `ApplicationState.IsLoggedIn` becomes `true`.
- The `CurrentUserChanged` event is raised.

## Password Management

Passwords are not editable directly.

When editing a user:

- Existing passwords remain unchanged.
- A new password is only stored when a value is entered in the password field.
- Leaving the password field empty preserves the currently stored password.

Passwords are encrypted using `IEncryptionService` before they are stored in the database.

## Application State

Authentication state is managed through `ApplicationState`.

### Properties

| Property | Description |
|-----------|-------------|
| IsAuthenticationRequired | Indicates whether authentication is required. |
| CurrentUser | Currently authenticated user. |
| IsLoggedIn | Indicates whether a user is authenticated. |

### Methods

```csharp
SetCurrentUser(User user);
Logout();
```

### Events

```csharp
CurrentUserChanged
```

## Initial Installation

For a fresh installation, authentication is disabled until at least one active user receives a password.

This allows administrators to create and configure initial user accounts before enforcing authentication.