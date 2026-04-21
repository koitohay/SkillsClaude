import { test, expect } from '@playwright/test'

const protectedClientRoutes = [
  '/dashboard',
  '/booking/new',
  '/booking/schedule',
  '/booking/review',
]

const protectedProviderRoutes = [
  '/provider/dashboard',
  '/provider/requests/some-id',
]

test.describe('Auth guards', () => {
  test.describe('Unauthenticated user', () => {
    for (const route of [...protectedClientRoutes, ...protectedProviderRoutes]) {
      test(`redirects unauthenticated user away from ${route}`, async ({ page }) => {
        await page.goto(route)
        await expect(page).toHaveURL(/auth\/login/)
      })
    }
  })

  test.describe('Client cannot access provider routes', () => {
    test.beforeEach(async ({ page, request }) => {
      const API = process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'
      const email = `client-guard-${Date.now()}@test.invalid`
      const res = await request.post(`${API}/auth/register/client`, {
        data: { fullName: 'Test', email, phoneNumber: '+4520999888', password: 'Test1234!' },
      })
      const { token, userId, role } = await res.json()
      await page.goto('/')
      await page.evaluate(({ token, userId, role }) => {
        localStorage.setItem('auth', JSON.stringify({ auth: { token, userId, role } }))
      }, { token, userId, role })
    })

    for (const route of protectedProviderRoutes) {
      test(`client is redirected from ${route}`, async ({ page }) => {
        await page.goto(route)
        await expect(page).not.toHaveURL(new RegExp(route.replace('/', '\\/').replace('[', '\\[').replace(']', '\\]')))
      })
    }
  })

  test('login page accessible to unauthenticated user', async ({ page }) => {
    await page.goto('/auth/login')
    await expect(page).toHaveURL(/auth\/login/)
    await expect(page.getByRole('heading')).toBeVisible()
  })

  test('register page accessible to unauthenticated user', async ({ page }) => {
    await page.goto('/auth/register')
    await expect(page).toHaveURL(/auth\/register/)
  })
})
