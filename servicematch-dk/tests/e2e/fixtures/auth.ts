import { test as base, type Page } from '@playwright/test'

const API_BASE = process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'

function uniqueEmail(prefix: string) {
  return `${prefix}-${Date.now()}@e2e-test.invalid`
}

async function registerClient(request: import('@playwright/test').APIRequestContext, email: string, password: string) {
  const res = await request.post(`${API_BASE}/auth/register/client`, {
    data: {
      fullName: 'Test Bruger',
      email,
      phoneNumber: '+4520123456',
      password,
    },
  })
  if (!res.ok()) throw new Error(`Client registration failed: ${await res.text()}`)
  return res.json() as Promise<{ token: string; userId: string; role: string }>
}

async function registerProvider(request: import('@playwright/test').APIRequestContext, email: string, password: string) {
  const res = await request.post(`${API_BASE}/auth/register/provider`, {
    data: {
      companyName: 'E2E Salon',
      contactName: 'Test Provider',
      email,
      phoneNumber: '+4520654321',
      cvrNumber: '12345678',
      address: 'Testgade 1',
      city: 'København',
      password,
      categoryIds: [1],
    },
  })
  if (!res.ok()) throw new Error(`Provider registration failed: ${await res.text()}`)
  return res.json() as Promise<{ token: string; userId: string; role: string }>
}

async function injectToken(page: Page, token: string, userId: string, role: string) {
  const maxAge = 60 * 60 * 24 * 7
  await page.context().addCookies([
    { name: 'auth_token', value: token, domain: 'localhost', path: '/', maxAge },
    { name: 'auth_role', value: role, domain: 'localhost', path: '/', maxAge },
    { name: 'auth_userId', value: userId, domain: 'localhost', path: '/', maxAge },
  ])
}

export type AuthFixtures = {
  clientPage: Page
  providerPage: Page
  clientEmail: string
  providerEmail: string
  clientToken: string
  providerToken: string
  providerId: string
}

export const test = base.extend<AuthFixtures>({
  clientEmail: async ({}, use) => {
    await use(uniqueEmail('client'))
  },

  providerEmail: async ({}, use) => {
    await use(uniqueEmail('provider'))
  },

  clientPage: async ({ browser, request, clientEmail }, use) => {
    const password = 'Test1234!'
    const auth = await registerClient(request, clientEmail, password)
    const ctx = await browser.newContext()
    const page = await ctx.newPage()
    await injectToken(page, auth.token, auth.userId, auth.role)
    await use(page)
    await ctx.close()
  },

  providerPage: async ({ browser, request, providerEmail }, use) => {
    const password = 'Test1234!'
    const auth = await registerProvider(request, providerEmail, password)
    const ctx = await browser.newContext()
    const page = await ctx.newPage()
    await injectToken(page, auth.token, auth.userId, auth.role)
    await use(page)
    await ctx.close()
  },

  clientToken: async ({ request, clientEmail }, use) => {
    const auth = await registerClient(request, clientEmail, 'Test1234!')
    await use(auth.token)
  },

  providerToken: async ({ request, providerEmail }, use) => {
    const auth = await registerProvider(request, providerEmail, 'Test1234!')
    await use(auth.token)
  },

  providerId: async ({ request, providerEmail }, use) => {
    const auth = await registerProvider(request, providerEmail, 'Test1234!')
    await use(auth.userId)
  },
})

export { expect } from '@playwright/test'
