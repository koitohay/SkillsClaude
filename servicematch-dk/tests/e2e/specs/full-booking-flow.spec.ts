import { test, expect } from '../fixtures/auth'
import { BookingNewPage, BookingSchedulePage, BookingReviewPage } from '../pages/BookingPage'

const tomorrow = () => {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  return d.toISOString().split('T')[0]
}

test.describe('Full booking flow', () => {
  test('client can create a request via category', async ({ clientPage }) => {
    const page = clientPage
    const newPage = new BookingNewPage(page)
    await newPage.goto()

    await newPage.selectCategory('Salon')
    await newPage.clickNext()

    const sched = new BookingSchedulePage(page)
    await sched.fillSchedule({ date: tomorrow(), time: '10:00', city: 'København' })
    await sched.clickNext()

    const review = new BookingReviewPage(page)
    const summary = await review.getSummaryText()
    expect(summary).toContain('Salon')
    expect(summary).toContain('København')

    await review.submit()
    await expect(page).toHaveURL(/\/booking\/.+\/offers/)
  })

  test('client can create a request via free text', async ({ clientPage }) => {
    const page = clientPage
    const newPage = new BookingNewPage(page)
    await newPage.goto()

    await newPage.enterFreeText('Akut rengøring af kontor')
    await newPage.clickNext()

    const sched = new BookingSchedulePage(page)
    await sched.fillSchedule({ date: tomorrow(), time: '14:00', city: 'Aarhus' })
    await sched.clickNext()

    const review = new BookingReviewPage(page)
    const summary = await review.getSummaryText()
    expect(summary).toContain('Aarhus')

    await review.submit()
    await expect(page).toHaveURL(/\/booking\/.+\/offers/)
  })

  test('rejects request with past date', async ({ clientPage }) => {
    const page = clientPage
    await new BookingNewPage(page).goto()
    await page.locator('[type="radio"], input[value="1"]').first().click()

    const yesterday = new Date()
    yesterday.setDate(yesterday.getDate() - 1)
    const dateStr = yesterday.toISOString().split('T')[0]

    await new BookingNewPage(page).clickNext()
    await new BookingSchedulePage(page).fillSchedule({ date: dateStr, time: '10:00', city: 'Odense' })
    await new BookingSchedulePage(page).clickNext()
    await new BookingReviewPage(page).submit()

    await expect(page).not.toHaveURL(/\/offers/)
  })

  test('client dashboard shows created request', async ({ clientPage }) => {
    const page = clientPage
    await new BookingNewPage(page).goto()
    await page.getByText('Massage', { exact: false }).click()
    await new BookingNewPage(page).clickNext()
    await new BookingSchedulePage(page).fillSchedule({ date: tomorrow(), time: '11:00', city: 'Odense' })
    await new BookingSchedulePage(page).clickNext()
    await new BookingReviewPage(page).submit()
    await expect(page).toHaveURL(/\/offers/)

    await page.goto('/dashboard')
    await expect(page.locator('.card').first()).toContainText('Massage')
  })
})
