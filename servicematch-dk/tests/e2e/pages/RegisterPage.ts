import type { Page } from '@playwright/test'

export class RegisterPage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/auth/register')
  }

  async registerClient(data: {
    fullName: string
    email: string
    phoneNumber: string
    password: string
  }) {
    await this.page.fill('[placeholder*="Navn"], input[name="fullName"], #fullName', data.fullName)
    await this.page.fill('[placeholder*="mail"], input[type="email"]', data.email)
    await this.page.fill('[placeholder*="tlf"], [placeholder*="45"], input[name="phoneNumber"]', data.phoneNumber)
    await this.page.fill('input[type="password"]', data.password)
    await this.page.getByRole('button', { name: /opret/i }).click()
  }

  async errorMessage() {
    return this.page.locator('[class*="text-red"]').first().textContent()
  }
}

export class RegisterProviderPage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/auth/register-provider')
  }

  async register(data: {
    companyName: string
    contactName: string
    email: string
    phoneNumber: string
    cvrNumber: string
    address: string
    city: string
    password: string
    categoryIds: number[]
  }) {
    await this.page.fill('input[name="companyName"], #companyName', data.companyName)
    await this.page.fill('input[name="contactName"], #contactName', data.contactName)
    await this.page.fill('input[type="email"]', data.email)
    await this.page.fill('input[name="phoneNumber"], #phoneNumber', data.phoneNumber)
    await this.page.fill('input[name="cvrNumber"], #cvrNumber', data.cvrNumber)
    await this.page.fill('input[name="address"], #address', data.address)
    await this.page.selectOption('select', data.city)
    await this.page.fill('input[type="password"]', data.password)

    for (const id of data.categoryIds) {
      await this.page.check(`input[type="checkbox"][value="${id}"]`)
    }

    await this.page.getByRole('button', { name: /opret/i }).click()
  }

  async errorMessage() {
    return this.page.locator('[class*="text-red"]').first().textContent()
  }
}
