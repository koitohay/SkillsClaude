# ServiceMatch DK — Development Standards

These rules apply to **every task** in this project without exception. Complete the relevant checklist items before marking any work done.

---

## Architecture Principles

- **KISS / YAGNI**: Don't add abstractions or features not explicitly requested. Three similar lines beat a premature helper.
- **Modular / Clean Architecture**: API → Application → Domain. No domain logic in controllers. No infrastructure imports in Application layer.
- **No hardcoded config**: All URLs, model names, timeouts, origins, expiry values belong in `appsettings.json` + bound `Options` classes. Never use `const string` for values that vary between environments.
- **No secrets in source**: JWT secrets, API keys, passwords must come from environment variables or Key Vault — never committed to git. `appsettings.Development.json` is gitignored.
- **Dependency injection over static**: No `new HttpClient()`, no static helpers for services.

---

## Before Marking Any Task Done — Required Checklist

### Backend changes
- [ ] New command/query handler has a corresponding unit test in `tests/ServiceMatch.Application.Tests/`
- [ ] New API endpoint has an integration test in `tests/ServiceMatch.API.Tests/`
- [ ] No new `const string` for config values — moved to `appsettings.json` + Options class
- [ ] No secrets added to any `appsettings*.json` file
- [ ] `dotnet build` passes with zero warnings
- [ ] `dotnet test` passes

### Frontend changes
- [ ] Component matches existing design system (see `assets/css/main.css` for shared classes)
- [ ] Mobile view tested (hamburger menu, responsive layout)
- [ ] No hardcoded API URLs — uses `$api()` composable
- [ ] Accessibility: interactive elements have accessible labels
- [ ] UI reviewed against design guidelines (see UI/UX section below)

### Infrastructure / DevOps changes
- [ ] New secrets added to `infra/modules/key-vault/main.tf` AND injected into Container App env
- [ ] Dockerfile changes maintain non-root `USER` directive
- [ ] Azure DevOps pipeline YAML updated if new env vars or build steps are needed
- [ ] Health check endpoints still functional after changes

### Playwright / E2E (when adding a new user-facing feature)
- [ ] New Playwright test file in `frontend/tests/e2e/` covering the happy path
- [ ] Test name maps to the user requirement it validates (e.g., `client-can-submit-service-request.spec.ts`)
- [ ] Tests pass with `npx playwright test`

---

## UI/UX Standards

Every UI change must be reviewed against these criteria before being marked complete:

1. **Visual hierarchy**: Headings, body, meta text use the correct CSS variables (`--text-1`, `--text-2`, `--text-3`)
2. **Buttons**: Use `btn-primary` / `btn-secondary` / `btn-ghost` / `btn-danger` — never raw `<button>` without a shared class
3. **Cards**: Use `card` or `card-hover` — consistent shadow/border/radius
4. **Forms**: All inputs use `input-field`; labels are visible (not just placeholder); error states are displayed
5. **Spacing**: Consistent use of Tailwind spacing scale — no magic pixel values
6. **Loading states**: Async operations show a spinner or skeleton, never a blank flash
7. **Empty states**: Lists with no data show a friendly empty-state message, not a blank area
8. **Mobile**: Test at 375px width — no horizontal overflow, no overlapping elements
9. **Color**: Use CSS variable palette (`--violet`, `--violet-lt`, `--surface`, `--border`) — no hardcoded hex except in `main.css`
10. **Accessibility**: Focus rings visible, `aria-label` on icon-only buttons, sufficient color contrast

---

## Reviewer Agent — When to Invoke

Run `/review` (the project-local skill) whenever:
- A feature is complete and about to be committed
- A backend service or domain entity has been added or significantly changed
- A UI page or component has been built or redesigned

The reviewer will check: architecture adherence, test coverage gaps, security issues, UI/UX quality, and Azure deployment impact.

---

## Project Tech Stack (reference)

- **Backend**: .NET 9, ASP.NET Core, EF Core 9, PostgreSQL (Npgsql), MediatR, FluentValidation, JWT, BCrypt
- **Frontend**: Nuxt 3, Vue 3, TypeScript, Tailwind CSS
- **AI**: Anthropic Claude via Novo Nordisk OpenAI-compatible gateway (`api.marketplace.novo-genai.com`)
- **Infrastructure**: Azure Container Apps, Azure Key Vault, Azure Container Registry, Terraform, Azure DevOps pipelines
- **Tests**: xUnit, Moq, FluentAssertions (backend); Playwright (frontend E2E)

---

## Key File Locations

| Concern | File |
|---|---|
| Shared CSS component classes | `frontend/assets/css/main.css` |
| API composable | `frontend/composables/useApi.ts` |
| Auth store | `frontend/stores/auth.ts` |
| DI registration | `backend/src/ServiceMatch.Infrastructure/DependencyInjection.cs` |
| App config | `backend/src/ServiceMatch.API/appsettings.json` |
| EF migrations | `backend/src/ServiceMatch.Infrastructure/Persistence/Migrations/` |
| Terraform | `infra/` |
| CI/CD pipelines | `pipelines/` |
| Integration test factory | `backend/tests/ServiceMatch.API.Tests/TestWebApplicationFactory.cs` |
