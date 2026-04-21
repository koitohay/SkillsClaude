import { test, expect } from '../fixtures/auth'

const API = () => process.env.API_BASE_URL ?? 'http://localhost:5000/api/v1'

const tomorrow = () => {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  return d.toISOString().split('T')[0]
}

// Email confirmations in dev/test are handled by LoggingEmailService (console output).
// These tests verify the API flows complete successfully and that the acceptance
// endpoint returns 200 — indicating email service was called without throwing.
// In a real email integration test environment, mailhog/mailpit would be used.

test.describe('Email confirmation (smoke)', () => {
  test('accepting an offer returns 200 (email service invoked without error)', async ({
    clientToken, providerToken, request
  }) => {
    // Create request
    const reqRes = await request.post(`${API()}/requests`, {
      headers: { Authorization: `Bearer ${clientToken}` },
      data: {
        categoryId: 1,
        requestedDate: tomorrow(),
        requestedTime: '09:00:00',
        city: 'København',
      },
    })
    expect(reqRes.status()).toBe(201)
    const { id: requestId } = await reqRes.json()

    // Provider submits offer
    const offerRes = await request.post(`${API()}/requests/${requestId}/offers`, {
      headers: { Authorization: `Bearer ${providerToken}` },
      data: { price: 350, message: 'Tilbud' },
    })
    expect(offerRes.status()).toBe(201)
    const { id: offerId } = await offerRes.json()

    // Client accepts — this triggers 2 confirmation emails via IEmailService
    const acceptRes = await request.put(`${API()}/requests/${requestId}/offers/${offerId}/accept`, {
      headers: { Authorization: `Bearer ${clientToken}` },
    })
    expect(acceptRes.status()).toBe(200)

    // Verify request is now Accepted
    const statusRes = await request.get(`${API()}/requests/${requestId}`, {
      headers: { Authorization: `Bearer ${clientToken}` },
    })
    const body = await statusRes.json()
    expect(body.status).toBe('Accepted')
  })
})
