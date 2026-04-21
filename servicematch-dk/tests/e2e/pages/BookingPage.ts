import type { Page } from '@playwright/test'

export class BookingNewPage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/booking/new')
  }

  async selectCategory(name: string) {
    await this.page.getByText(name, { exact: false }).click()
  }

  async enterFreeText(text: string) {
    await this.page.getByRole('button', { name: /fri tekst/i }).click()
    await this.page.fill('textarea', text)
  }

  async clickNext() {
    await this.page.getByRole('button', { name: /næste/i }).click()
  }
}

export class BookingSchedulePage {
  constructor(private page: Page) {}

  async fillSchedule(data: { date: string; time: string; city: string }) {
    await this.page.fill('input[type="date"]', data.date)
    await this.page.fill('input[type="time"]', data.time)
    await this.page.selectOption('select', data.city)
  }

  async clickNext() {
    await this.page.getByRole('button', { name: /næste/i }).click()
  }
}

export class BookingReviewPage {
  constructor(private page: Page) {}

  async submit() {
    await this.page.getByRole('button', { name: /send/i }).click()
  }

  async getSummaryText() {
    return this.page.locator('.card').textContent()
  }
}
