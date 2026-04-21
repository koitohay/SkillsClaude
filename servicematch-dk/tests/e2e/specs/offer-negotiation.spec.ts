import { test, expect } from '../fixtures/auth'
import { BookingNewPage, BookingSchedulePage, BookingReviewPage } from '../pages/BookingPage'
import { OffersPage } from '../pages/OffersPage'
import { ProviderDashboardPage, ProviderRequestDetailPage } from '../pages/ProviderRequestPage'

const API = () => process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'

const tomorrow = () => {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  return d.toISOString().split('T')[0]
}

async function createRequestViaApi(token: string, request: import('@playwright/test').APIRequestContext) {
  const res = await request.post(`${API()}/requests`, {
    headers: { Authorization: `Bearer ${token}` },
    data: {
      categoryId: 1,
      requestedDate: tomorrow(),
      requestedTime: '10:00:00',
      city: 'København',
    },
  })
  const body = await res.json()
  return body.id as string
}

async function submitOfferViaApi(token: string, requestId: string, price: number, request: import('@playwright/test').APIRequestContext) {
  const res = await request.post(`${API()}/requests/${requestId}/offers`, {
    headers: { Authorization: `Bearer ${token}` },
    data: { price, message: 'Vi tilbyder en god pris.' },
  })
  const body = await res.json()
  return body.id as string
}

test.describe('Offer negotiation', () => {
  test('provider submits offer and client accepts', async ({ clientPage, providerPage, clientToken, providerToken, request }) => {
    const reqId = await createRequestViaApi(clientToken, request)
    await submitOfferViaApi(providerToken, reqId, 450, request)

    await clientPage.goto(`/booking/${reqId}/offers`)
    const offersPage = new OffersPage(clientPage)
    await offersPage.waitForOffers()

    await expect(clientPage.locator('text=450')).toBeVisible()
    await offersPage.acceptFirstOffer()

    await clientPage.waitForTimeout(500)
    const status = await offersPage.getStatusBadge()
    expect(status).toMatch(/accepteret/i)
  })

  test('client declines offer', async ({ clientPage, providerToken, clientToken, request }) => {
    const reqId = await createRequestViaApi(clientToken, request)
    await submitOfferViaApi(providerToken, reqId, 600, request)

    await clientPage.goto(`/booking/${reqId}/offers`)
    const offersPage = new OffersPage(clientPage)
    await offersPage.waitForOffers()
    await offersPage.declineFirstOffer()

    await clientPage.waitForTimeout(500)
    const status = await offersPage.getStatusBadge()
    expect(status).toMatch(/afvist/i)
  })

  test('full counter-offer chain: client counters, provider accepts', async ({
    clientPage, providerPage, clientToken, providerToken, request
  }) => {
    const reqId = await createRequestViaApi(clientToken, request)
    await submitOfferViaApi(providerToken, reqId, 500, request)

    // Client sends counter offer
    await clientPage.goto(`/booking/${reqId}/offers`)
    const offersPage = new OffersPage(clientPage)
    await offersPage.waitForOffers()
    await offersPage.openCounterModal()
    await offersPage.fillCounter(400, 'Kan vi gøre det for 400?')
    await offersPage.submitCounter()

    await clientPage.waitForTimeout(500)
    const statusAfterCounter = await offersPage.getStatusBadge()
    expect(statusAfterCounter).toMatch(/modtilbud/i)

    // Provider accepts the client counter
    await providerPage.goto(`/provider/requests/${reqId}`)
    const providerDetail = new ProviderRequestDetailPage(providerPage)
    await providerPage.waitForTimeout(500)
    await providerDetail.acceptNegotiation()

    await providerPage.waitForTimeout(500)
    const providerStatus = await providerDetail.getOfferStatus()
    expect(providerStatus).toMatch(/accepteret/i)
  })

  test('provider submits offer via dashboard', async ({ clientToken, providerPage, providerToken, request }) => {
    const reqId = await createRequestViaApi(clientToken, request)

    const dashboard = new ProviderDashboardPage(providerPage)
    await dashboard.goto()
    await expect(providerPage.locator('.card').first()).toBeVisible()
    await dashboard.openFirstRequest()

    const detail = new ProviderRequestDetailPage(providerPage)
    await detail.submitOffer(380, 'Tilbud fra dashboard')

    await providerPage.waitForTimeout(500)
    await expect(providerPage.locator('text=380')).toBeVisible()
  })
})
