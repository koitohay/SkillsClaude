import type { Page } from '@playwright/test'

export class ProviderDashboardPage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/provider/dashboard')
  }

  async getRequestCount() {
    return this.page.locator('.card').count()
  }

  async openFirstRequest() {
    await this.page.getByRole('link', { name: /se detaljer/i }).first().click()
  }
}

export class ProviderRequestDetailPage {
  constructor(private page: Page) {}

  async submitOffer(price: number, message?: string) {
    await this.page.fill('input[type="number"]', String(price))
    if (message) {
      await this.page.fill('textarea', message)
    }
    await this.page.getByRole('button', { name: /send tilbud/i }).click()
  }

  async acceptNegotiation() {
    await this.page.getByRole('button', { name: /accepter/i }).click()
  }

  async declineNegotiation() {
    await this.page.getByRole('button', { name: /afvis/i }).click()
  }

  async openProviderCounter() {
    await this.page.getByRole('button', { name: /send modtilbud/i }).click()
  }

  async fillProviderCounter(price: number, message?: string) {
    await this.page.fill('input[placeholder*="modpris"]', String(price))
    if (message) {
      await this.page.fill('textarea[placeholder*="Besked"]', message)
    }
    await this.page.getByRole('button', { name: /send modtilbud/i }).last().click()
  }

  async getOfferStatus() {
    return this.page.locator('[class*="rounded-full"]').last().textContent()
  }
}
