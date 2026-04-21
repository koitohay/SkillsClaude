import { test, expect } from '@playwright/test'
import { RegisterProviderPage } from '../pages/RegisterPage'

const validProvider = (overrides: Partial<Parameters<RegisterProviderPage['register']>[0]> = {}) => ({
  companyName: 'Salon Test',
  contactName: 'Lars Larsen',
  email: `provider-${Date.now()}@test.invalid`,
  phoneNumber: '+4540123456',
  cvrNumber: '12345678',
  address: 'Bredgade 1',
  city: 'København',
  password: 'Test1234!',
  categoryIds: [1],
  ...overrides,
})

test.describe('Provider registration', () => {
  test('registers with valid CVR and category', async ({ page }) => {
    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider())
    await expect(page).not.toHaveURL(/auth\/register-provider/)
  })

  test('rejects CVR with wrong length', async ({ page }) => {
    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider({ cvrNumber: '1234' }))
    const err = await reg.errorMessage()
    expect(err).toBeTruthy()
  })

  test('rejects all-zeros CVR', async ({ page }) => {
    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider({ cvrNumber: '00000000' }))
    const err = await reg.errorMessage()
    expect(err).toBeTruthy()
  })

  test('rejects duplicate email', async ({ page, request }) => {
    const email = `dup-prov-${Date.now()}@test.invalid`
    await request.post(`${process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'}/auth/register/provider`, {
      data: validProvider({ email, cvrNumber: '87654321' }),
    })

    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider({ email, cvrNumber: '11223344' }))

    const err = await reg.errorMessage()
    expect(err).toMatch(/email|allerede/i)
  })

  test('rejects duplicate CVR', async ({ page, request }) => {
    const cvr = '55667788'
    await request.post(`${process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'}/auth/register/provider`, {
      data: validProvider({ cvrNumber: cvr }),
    })

    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider({ email: `cvr-dup-${Date.now()}@test.invalid`, cvrNumber: cvr }))

    const err = await reg.errorMessage()
    expect(err).toMatch(/cvr|allerede/i)
  })

  test('can select multiple service categories', async ({ page }) => {
    const reg = new RegisterProviderPage(page)
    await reg.goto()
    await reg.register(validProvider({ categoryIds: [1, 2, 3] }))
    await expect(page).not.toHaveURL(/auth\/register-provider/)
  })
})
