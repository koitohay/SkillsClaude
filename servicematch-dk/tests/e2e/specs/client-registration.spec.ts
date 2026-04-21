import { test, expect } from '@playwright/test'
import { RegisterPage } from '../pages/RegisterPage'

test.describe('Client registration', () => {
  test('registers with valid Danish phone number', async ({ page }) => {
    const reg = new RegisterPage(page)
    await reg.goto()
    await reg.registerClient({
      fullName: 'Anna Jensen',
      email: `anna-${Date.now()}@test.invalid`,
      phoneNumber: '+4520123456',
      password: 'Test1234!',
    })
    await expect(page).not.toHaveURL(/login/)
  })

  test('accepts 8-digit DK phone without country code', async ({ page }) => {
    const reg = new RegisterPage(page)
    await reg.goto()
    await reg.registerClient({
      fullName: 'Bo Nielsen',
      email: `bo-${Date.now()}@test.invalid`,
      phoneNumber: '20123456',
      password: 'Test1234!',
    })
    await expect(page).not.toHaveURL(/auth\/register/)
  })

  test('rejects invalid phone number', async ({ page }) => {
    const reg = new RegisterPage(page)
    await reg.goto()
    await reg.registerClient({
      fullName: 'Clara Hansen',
      email: `clara-${Date.now()}@test.invalid`,
      phoneNumber: '1234',
      password: 'Test1234!',
    })
    await expect(page).toHaveURL(/auth\/register/)
    const err = await reg.errorMessage()
    expect(err).toBeTruthy()
  })

  test('rejects duplicate email', async ({ page, request }) => {
    const email = `dup-${Date.now()}@test.invalid`
    await request.post(`${process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'}/auth/register/client`, {
      data: { fullName: 'First', email, phoneNumber: '+4530123456', password: 'Test1234!' },
    })

    const reg = new RegisterPage(page)
    await reg.goto()
    await reg.registerClient({ fullName: 'Second', email, phoneNumber: '+4530123457', password: 'Test1234!' })

    const err = await reg.errorMessage()
    expect(err).toMatch(/email|allerede/i)
  })
})
