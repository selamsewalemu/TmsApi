# TmsApi

A Training Management System (TMS) REST API built with ASP.NET Core (.NET 10). It manages student course enrollments and exposes a clean HTTP API with OpenAPI documentation.

## Tech Stack

- **Runtime:** .NET 10
- **Framework:** ASP.NET Core (Minimal APIs + Controllers)
- **API Docs:** OpenAPI via `Microsoft.AspNetCore.OpenApi` + Scalar UI
- **Auth:** Custom test authentication scheme (pluggable)

## Project Structure

```
TmsApi/
├── Controllers/
│   └── EnrollmentsController.cs   # CRUD endpoints for enrollments
├── EnrollmentService.cs           # In-memory enrollment store + business logic
├── EnrollmentWorker.cs            # Background batch processor (singleton)
├── PaymentOptions.cs              # Strongly-typed payment config with validation
├── RequestLoggingMiddleware.cs    # Per-request logging with correlation IDs
├── TmsDatabaseException.cs        # Custom domain exception
├── Program.cs                     # App bootstrap and minimal API routes
└── appsettings.json               # App configuration
```

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)

### Run the API

```bash
dotnet run
```

The API starts on:
- HTTP: `http://localhost:5013`
- HTTPS: `https://localhost:7216`

### API Documentation

When running in Development, the interactive Scalar UI is available at:

```
http://localhost:5013/scalar
```

OpenAPI spec is served at:

```
http://localhost:5013/openapi/v1.json
```

## API Endpoints

### Enrollments

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/enrollments` | List all enrollment records |
| `GET` | `/api/enrollments/{id}` | Get a single enrollment by ID |
| `POST` | `/api/enrollments` | Enroll a student in a course |
| `DELETE` | `/api/enrollments/{id}` | Remove an enrollment |

#### POST `/api/enrollments` — Request Body

```json
{
  "studentId": "S-001",
  "courseCode": "CS-101"
}
```

#### Enrollment Record Response

```json
{
  "id": "a1b2c3d4",
  "studentId": "S-001",
  "courseCode": "CS-101",
  "enrolledAt": "2026-09-02T10:00:00Z"
}
```

Duplicate enrollment attempts return the existing record without creating a new one.

### Assessment Results (Protected)

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/assessments/results` | Get assessment results (requires auth) |

### Utility

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/enrollments/worker-smoke` | Trigger a batch processing smoke test |
| `GET` | `/api/error` | Simulate a database error (ProblemDetails testing) |

## Configuration

Settings are defined in `appsettings.json`. The `Payments` section is bound to `PaymentOptions` and validated on startup.

```json
{
  "Payments": {
    "GatewayUrl": "https://payments.example.test",
    "MaxDepositBirr": 5000
  }
}
```

| Key | Type | Constraints | Description |
|-----|------|-------------|-------------|
| `GatewayUrl` | `string` | Required | Payment gateway base URL |
| `MaxDepositBirr` | `decimal` | 100–100,000 | Maximum allowed deposit in Birr |

## Features

- **Request Logging Middleware** — logs method, path, status code, duration (ms), and a short correlation ID (`X-Correlation-Id` response header) for every request.
- **ProblemDetails** — structured RFC 9457 error responses out of the box.
- **Strongly-typed options** — `PaymentOptions` is validated with data annotations at startup, so misconfiguration fails fast.
- **Scoped DI safety** — `ValidateScopes` and `ValidateOnBuild` are enabled to catch DI lifetime mismatches during development.
- **Duplicate enrollment guard** — re-enrolling the same student in the same course returns the existing record and logs a warning instead of creating a duplicate.

## Development Notes

- The enrollment store is **in-memory** — data does not persist across restarts. Replace `EnrollmentService` with a database-backed implementation for production use.
- The authentication handler always fails (`AuthenticateResult.Fail`). Swap it with a real JWT or API key scheme before deploying.
- `EnrollmentWorker` is registered as a singleton and resolves `IEnrollmentService` (scoped) via `IServiceScopeFactory` to avoid captive dependency issues.

## License

MIT
