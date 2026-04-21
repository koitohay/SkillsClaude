import type { Page } from '@playwright/test'

export class OffersPage {
  constructor(private page: Page) {}

  async waitForOffers() {
    await this.page.waitForSelector('.card', { timeout: 10000 })
  }

  async getOfferCount() {
    return this.page.locator('.card').count()
  }

  async acceptFirstOffer() {
    await this.page.getByRole('button', { name: /accepter/i }).first().click()
  }

  async declineFirstOffer() {
    await this.page.getByRole('button', { name: /afvis/i }).first().click()
  }

  async openCounterModal() {
    await this.page.getByRole('button', { name: /modtilbud/i }).first().click()
  }

  async fillCounter(price: number, message?: string) {
    await this.page.fill('input[type="number"]', String(price))
    if (message) {
      await this.page.fill('textarea', message)
    }
  }

  async submitCounter() {
    await this.page.getByRole('button', { name: /send modtilbud/i }).click()
  }

  async getStatusBadge(index = 0) {
    return this.page.locator('[class*="rounded-full"]').nth(index).textContent()
  }
}
