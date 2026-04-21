# ServiceMatch DK

A service booking platform for the Danish market. Clients request services (salon, massage, tandlæge, etc.), providers respond with price offers, and both parties negotiate until agreed.

## Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Vue 3 + Nuxt 3 (SSR), Pinia, Tailwind CSS |
| Backend | .NET 9 ASP.NET Core, Clean Architecture, MediatR, FluentValidation, EF Core 9 |
| Database | PostgreSQL 16 |
| Email | Azure Communication Services (`LoggingEmailService` in dev) |
| Auth | JWT Bearer (roles: `Client` \| `Provider`) |
| IaC | Terraform 1.7+ (Azure) |
| CI/CD | Azure DevOps YAML pipelines |
| Testing | xUnit + NSubstitute + Testcontainers, Playwright E2E |

## Quick start (local)

**Prerequisites**: Docker Desktop, .NET 9 SDK, Node 22.

```bash
# 1. Start PostgreSQL
docker-compose up postgres -d

# 2. Run database migrations + seed data
cd backend
dotnet run --project src/ServiceMatch.API

# 3. Start frontend (separate terminal)
cd frontend
cp .env.example .env
npm install
npm run dev
```

Open http://localhost:3000. The seeded providers use password `Provider123!`.

**Full local stack via docker-compose:**

```bash
docker-compose up --build
```

## Running tests

```bash
# Unit + application tests (no infra needed)
cd backend
dotnet test --filter "FullyQualifiedName!~Integration"

# Integration tests (requires Docker for Testcontainers)
dotnet test --filter "FullyQualifiedName~Integration"

# All tests
dotnet test

# E2E tests (requires running local stack)
cd tests/e2e
npm ci
npx playwright install --with-deps chromium
BASE_URL=http://localhost:3000 npx playwright test
```

## Project structure

```
servicematch-dk/
├── backend/
│   ├── src/
│   │   ├── ServiceMatch.Domain/        # Entities, value objects, enums
│   │   ├── ServiceMatch.Application/   # CQRS handlers, DTOs, validators
│   │   ├── ServiceMatch.Infrastructure/# EF Core, repos, services, migrations
│   │   └── ServiceMatch.API/           # Controllers, middleware, Program.cs
│   └── tests/
│       ├── ServiceMatch.Domain.Tests/
│       ├── ServiceMatch.Application.Tests/
│       └── ServiceMatch.Integration.Tests/
├── frontend/                           # Nuxt 3 app
├── infrastructure/                     # Terraform modules + env tfvars
├── tests/e2e/                          # Playwright specs (POM pattern)
└── .azuredevops/pipelines/             # 6 Azure DevOps YAML pipelines
```

## API

Base URL: `/api/v1`

| Method | Path | Auth |
|--------|------|------|
| POST | `/auth/register/client` | None |
| POST | `/auth/register/provider` | None |
| POST | `/auth/login` | None |
| GET | `/categories` | None |
| POST/GET | `/requests` | Client |
| GET | `/requests/{id}` | Client |
| POST/GET | `/requests/{id}/offers` | Provider (POST) / Client (GET) |
| PUT | `/requests/{id}/offers/{id}/accept\|decline` | Client |
| POST | `/requests/{id}/offers/{id}/counter` | Client |
| PUT | `/requests/{id}/offers/{id}/negotiations/{nId}/accept\|decline\|counter` | Provider |
| GET | `/providers/me/requests` | Provider |
| GET | `/health` | None |

Interactive Swagger docs available at `/swagger` in Development.

## Infrastructure

Terraform modules under `infrastructure/modules/`:
- `postgresql` — PostgreSQL Flexible Server
- `key-vault` — Key Vault with all app secrets
- `container-app` — Container App Environment + API Container App
- `static-web-app` — Frontend Container App
- `application-insights` — Log Analytics + Application Insights
- `communication-services` — Azure Communication Services

```bash
cd infrastructure
cp terraform.tfvars.example terraform.tfvars  # fill in secrets
terraform init -backend-config="..."
terraform plan -var-file=environments/dev.tfvars
terraform apply -var-file=environments/dev.tfvars
```

## CI/CD Pipelines

| Pipeline | Trigger | Purpose |
|----------|---------|---------|
| `ci-backend` | Push to main/develop (backend changes) | Build, test, push API image to ACR |
| `ci-frontend` | Push to main/develop (frontend changes) | Build, typecheck, push frontend image |
| `cd-backend` | ci-backend succeeds on main | Deploy API: dev (auto) → test (1 approver) → prod (2 approvers + 30 min bake) |
| `cd-frontend` | ci-frontend succeeds on main | Deploy frontend: same 3-stage pattern |
| `infrastructure` | Push to main/develop (infra changes) | terraform validate + plan (always), apply (manual gates) |
| `e2e-tests` | Triggered by cd-backend after test deploy | Playwright tests on test environment |

## Seed data

Five providers are seeded on startup:

| Provider | City | Categories |
|----------|------|------------|
| Salon Bella | København | Salon |
| Wellness Aarhus | Aarhus | Massage |
| Odense Tandklinik | Odense | Tandlæge |
| Aalborg Kiropraktik | Aalborg | Kiropraktor |
| Esbjerg Negle & Massage | Esbjerg | Negle, Massage |

Default password: `Provider123!`
