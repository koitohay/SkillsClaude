import type { Page } from '@playwright/test'

export class LoginPage {
  constructor(private page: Page) {}

  async goto() {
    await this.page.goto('/auth/login')
  }

  async login(email: string, password: string) {
    await this.page.fill('input[type="email"]', email)
    await this.page.fill('input[type="password"]', password)
    await this.page.getByRole('button', { name: /log ind/i }).click()
  }

  async errorMessage() {
    return this.page.locator('[class*="text-red"]').first().textContent()
  }
}
