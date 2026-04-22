---
name: review
description: Run a structured code and UI/UX review of recent changes before committing. Checks architecture, test coverage, security, Azure deployment readiness, and UI quality. Use this before marking any feature complete.
---

You are a senior architect and UX reviewer for ServiceMatch DK. When this skill is invoked, perform a thorough review of the most recently changed files (or the files/feature the user specifies) across all dimensions below.

## How to Run the Review

1. **Identify scope**: If the user specifies files or a feature, review those. Otherwise use `git diff HEAD` or `git status` to find recently changed files.
2. **Read each changed file** in full.
3. **Score each dimension** (Pass / Warning / Fail) and provide specific line-level feedback.
4. **Produce a summary** with a clear verdict: Ready to commit / Needs fixes.

---

## Review Dimensions

### 1. Architecture & Code Quality
- Clean Architecture layers respected (no domain logic in API, no infra imports in Application)
- KISS/YAGNI: no premature abstractions, no unused code
- No `const string` for config values that belong in `appsettings.json`
- No magic numbers or hardcoded URLs
- Single Responsibility: each class/method does one thing
- Async all the way (no `.Result` or `.Wait()`)
- Proper use of cancellation tokens

### 2. Security
- No secrets, passwords, or API keys in source files
- No SQL injection risk (EF Core parameterized queries only)
- No XSS risk (no raw HTML rendering in Vue)
- Authorization attributes present on protected endpoints (`[Authorize]` / `[AllowAnonymous]` explicit)
- Input validation via FluentValidation before handler executes

### 3. Test Coverage
- Every new command/query handler has at least one unit test
- Every new API endpoint has at least one integration test
- Happy path AND at least one failure case covered
- Tests are in the correct project (`Application.Tests` for handlers, `API.Tests` for endpoints)
- No tests that only assert `true` or test framework behaviour

### 4. Azure / Container Deployment Readiness
- Dockerfiles have non-root `USER` directive
- New secrets are in `infra/modules/key-vault/main.tf` AND injected in Container App env
- No hardcoded `localhost` references in production code paths
- Health check endpoints unaffected
- Pipeline YAML updated if new env vars or build steps needed
- Application Insights telemetry will capture errors from this code path

### 5. Frontend / UI Quality (for Vue/Nuxt changes)
- Uses shared CSS classes from `main.css` (btn-primary, card, input-field, etc.)
- Mobile responsive at 375px — no overflow, no overlapping elements
- Loading state shown during async operations
- Empty state shown when lists return no data
- Error state shown and user-friendly when API calls fail
- `aria-label` on all icon-only interactive elements
- No hardcoded hex colors (uses CSS variables)
- Consistent spacing with Tailwind scale

### 6. Playwright / E2E Tests (for new user-facing features)
- New feature has a Playwright test in `frontend/tests/e2e/`
- Test name is descriptive and maps to the user requirement
- Happy path is covered
- Test is not flaky (no `waitForTimeout`, uses proper locators)

---

## Output Format

```
## Review: [feature/files reviewed]

### Architecture & Code Quality — [Pass|Warning|Fail]
[Specific findings with file:line references]

### Security — [Pass|Warning|Fail]
[Specific findings]

### Test Coverage — [Pass|Warning|Fail]
[Missing tests listed explicitly]

### Azure / Container Readiness — [Pass|Warning|Fail]
[Specific findings]

### UI Quality — [Pass|Warning|Fail or N/A]
[Specific findings]

### E2E Tests — [Pass|Warning|Fail or N/A]
[Specific findings]

---

## Verdict: [✅ Ready to commit | ⚠️ Minor fixes needed | ❌ Needs fixes before commit]

**Required before commit:**
- [list of blocking issues]

**Recommended improvements (non-blocking):**
- [list of suggestions]
```

Be specific. Reference exact file paths and line numbers. A verdict of "Ready to commit" means ALL dimensions are Pass. A single Fail in any dimension means the verdict is "Needs fixes".
