<template>
  <div class="max-w-2xl mx-auto">
    <button @click="$router.back()" class="btn-ghost mb-6 -ml-2">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/></svg>
      Tilbage
    </button>

    <!-- Loading -->
    <div v-if="pending" class="card flex items-center justify-center py-16">
      <div class="w-6 h-6 border-2 border-t-transparent rounded-full animate-spin" style="border-color: var(--violet-lt); border-top-color: transparent;"></div>
    </div>

    <template v-else-if="request">

      <!-- Page header -->
      <div class="flex items-center justify-between mb-6">
        <div>
          <h2 class="page-header">Forespørgsel</h2>
          <p class="page-sub">{{ request.categoryName ?? 'Fri tekst forespørgsel' }}</p>
        </div>
        <span :class="statusBadge(request.status)">{{ statusLabel(request.status) }}</span>
      </div>

      <!-- Request details card -->
      <div class="card mb-5">
        <p class="section-label">Detaljer</p>
        <div class="space-y-3">
          <div class="flex justify-between items-center">
            <span class="text-sm" style="color: var(--text-2);">Kategori</span>
            <span class="font-semibold text-sm" style="color: var(--text-1);">{{ request.categoryName ?? 'Fri tekst' }}</span>
          </div>
          <div v-if="request.freeTextDescription" class="flex justify-between items-start gap-4">
            <span class="text-sm flex-shrink-0" style="color: var(--text-2);">Beskrivelse</span>
            <span class="font-medium text-sm text-right leading-snug" style="color: var(--text-1); max-width: 280px;">{{ request.freeTextDescription }}</span>
          </div>
          <div style="border-top: 1.5px solid var(--border); margin: 0.75rem 0;"></div>
          <div class="grid grid-cols-3 gap-3 text-center">
            <div class="rounded-xl p-3" style="background: var(--bg); border: 1.5px solid var(--border);">
              <p class="text-xs mb-1" style="color: var(--text-3);">Dato</p>
              <p class="font-semibold text-sm" style="color: var(--text-1);">{{ request.requestedDate }}</p>
            </div>
            <div class="rounded-xl p-3" style="background: var(--bg); border: 1.5px solid var(--border);">
              <p class="text-xs mb-1" style="color: var(--text-3);">Tidspunkt</p>
              <p class="font-semibold text-sm" style="color: var(--text-1);">{{ request.requestedTime.slice(0, 5) }}</p>
            </div>
            <div class="rounded-xl p-3" style="background: var(--bg); border: 1.5px solid var(--border);">
              <p class="text-xs mb-1" style="color: var(--text-3);">By</p>
              <p class="font-semibold text-sm" style="color: var(--text-1);">{{ request.city }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- ── Existing offer from this provider ── -->
      <div v-if="myOffer" class="card mb-5">
        <div class="flex items-center justify-between mb-5">
          <p class="section-label mb-0">Dit tilbud</p>
          <span :class="offerStatusBadge(myOffer.status)">{{ offerStatusLabel(myOffer.status) }}</span>
        </div>

        <!-- Price display -->
        <div class="rounded-2xl px-6 py-5 mb-4 flex items-baseline gap-2"
             style="background: linear-gradient(135deg, rgb(237 233 254 / 0.6) 0%, rgb(221 214 254 / 0.3) 100%); border: 1.5px solid #c4b5fd;">
          <span class="text-4xl font-bold leading-none" style="color: var(--violet-lt); letter-spacing: -0.04em;">{{ myOffer.price.toFixed(0) }}</span>
          <span class="text-base font-medium" style="color: var(--violet-lt); opacity: 0.7;">DKK</span>
        </div>

        <p v-if="myOffer.message" class="text-sm leading-relaxed mb-4" style="color: var(--text-2);">{{ myOffer.message }}</p>

        <!-- Negotiation thread -->
        <div v-if="myOffer.negotiations?.length" class="mb-4">
          <p class="section-label mb-3">Forhandlingshistorik</p>
          <div class="space-y-2">
            <div v-for="n in myOffer.negotiations" :key="n.id"
                 class="rounded-xl px-4 py-3"
                 :style="n.initiatedBy === 'Provider'
                   ? 'background: rgb(237 233 254 / 0.5); border: 1.5px solid #c4b5fd; margin-right: 2rem;'
                   : 'background: var(--bg); border: 1.5px solid var(--border); margin-left: 2rem;'">
              <div class="flex items-center justify-between mb-1">
                <span class="text-xs font-semibold"
                      :style="n.initiatedBy === 'Provider' ? 'color: var(--violet-lt);' : 'color: var(--text-2);'">
                  {{ n.initiatedBy === 'Provider' ? 'Dig' : 'Klient' }}
                </span>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold" style="color: var(--text-1);">{{ n.proposedPrice.toFixed(0) }} DKK</span>
                  <span :class="negStatusBadge(n.status)">{{ negStatusLabel(n.status) }}</span>
                </div>
              </div>
              <p v-if="n.message" class="text-xs leading-relaxed" style="color: var(--text-2);">"{{ n.message }}"</p>
            </div>
          </div>
        </div>

        <!-- Pending client counter — action required -->
        <div v-if="pendingClientNegotiation"
             class="rounded-2xl p-5"
             style="background: rgb(254 243 199 / 0.5); border: 1.5px solid #fcd34d;">

          <p class="text-xs font-bold uppercase tracking-wider mb-3" style="color: #92400e;">Klientens forslag</p>
          <div class="flex items-baseline gap-2 mb-2">
            <span class="text-3xl font-bold leading-none" style="color: #78350f; letter-spacing: -0.04em;">
              {{ pendingClientNegotiation.proposedPrice.toFixed(0) }}
            </span>
            <span class="text-sm font-medium" style="color: #92400e;">DKK</span>
          </div>
          <p v-if="pendingClientNegotiation.message" class="text-sm italic mb-4" style="color: #92400e;">
            "{{ pendingClientNegotiation.message }}"
          </p>

          <div v-if="negError" class="alert-error mb-3">
            <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
            {{ negError }}
          </div>

          <div class="flex gap-2 mb-3">
            <button @click="acceptNegotiation" class="btn-primary flex-1">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/></svg>
              Accepter pris
            </button>
            <button @click="declineNegotiation" class="btn-danger flex-1">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
              Afvis
            </button>
          </div>

          <!-- Provider counter form -->
          <div v-if="showProviderCounter" class="space-y-3 pt-3" style="border-top: 1.5px solid #fcd34d;">
            <div class="relative">
              <input v-model.number="counterForm.price" type="number" min="1" class="input-field pr-14" placeholder="Dit modtilbud" />
              <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm font-medium" style="color: var(--text-3);">DKK</span>
            </div>
            <textarea v-model="counterForm.message" rows="2" class="input-field" placeholder="Besked (valgfri)" />
            <div class="flex gap-2">
              <button @click="submitProviderCounter" class="btn-primary flex-1">Send modtilbud</button>
              <button @click="showProviderCounter = false" class="btn-secondary px-4">Annuller</button>
            </div>
          </div>
          <button v-else @click="showProviderCounter = true" class="btn-ghost text-xs w-full justify-center mt-1">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"/></svg>
            Foreslå anden pris
          </button>
        </div>
      </div>

      <!-- ── Submit new offer ── -->
      <div v-else-if="request.status === 'Open' || request.status === 'OfferReceived'" class="card">
        <p class="section-label">Afgiv tilbud</p>
        <p class="text-sm leading-relaxed mb-6" style="color: var(--text-2);">
          Indgiv din pris for at besvare denne forespørgsel. Klienten kan acceptere, afvise eller forhandle.
        </p>

        <div v-if="submitError" class="alert-error mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          {{ submitError }}
        </div>

        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">Din pris</label>
            <div class="relative">
              <input v-model.number="offerForm.price" type="number" min="1"
                     class="input-field pr-14 text-xl font-bold" placeholder="0"
                     style="letter-spacing: -0.02em;" />
              <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm font-medium" style="color: var(--text-3);">DKK</span>
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium mb-2" style="color: var(--text-1);">
              Besked til klienten
              <span class="font-normal" style="color: var(--text-3);">(valgfri)</span>
            </label>
            <textarea v-model="offerForm.message" rows="3" class="input-field" placeholder="Beskriv hvad din pris inkluderer…" />
          </div>
        </div>

        <button @click="submitOffer" :disabled="submitting || offerForm.price <= 0" class="btn-primary mt-5 w-full py-3">
          <svg v-if="submitting" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
          <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"/></svg>
          {{ submitting ? 'Sender…' : 'Send tilbud' }}
        </button>
      </div>

      <!-- Booking confirmed -->
      <div v-else-if="request.status === 'Accepted'" class="card text-center py-10">
        <div class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-4"
             style="background: #d1fae5;">
          <svg class="w-7 h-7" style="color: #059669;" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
        </div>
        <p class="font-semibold mb-1" style="color: var(--text-1);">Booking bekræftet</p>
        <p class="text-sm" style="color: var(--text-2);">Denne forespørgsel er accepteret og er nu en booking.</p>
        <NuxtLink to="/provider/bookings" class="btn-primary mt-5">Se mine bookinger</NuxtLink>
      </div>
    </template>

    <div v-else class="card text-center py-12">
      <p class="font-semibold" style="color: var(--text-1);">Forespørgsel ikke fundet.</p>
      <button @click="$router.back()" class="btn-secondary mt-4">Gå tilbage</button>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'provider-only' })

import type { ServiceRequestDto, ServiceRequestStatus, OfferStatus, NegotiationStatus } from '~/types/api.types'

const { $api } = useNuxtApp()
const route = useRoute()
const authStore = useAuthStore()
const requestId = route.params.id as string

const { data: request, pending, refresh } = await useAsyncData(`provider-request-${requestId}`, () =>
  $api<ServiceRequestDto>(`/providers/me/requests/${requestId}`))

const myOffer = computed(() =>
  request.value?.offers?.find(o => o.serviceProviderId === authStore.userId) ?? null)

const pendingClientNegotiation = computed(() => {
  if (!myOffer.value) return null
  return myOffer.value.negotiations?.find(n => n.initiatedBy === 'Client' && n.status === 'Pending') ?? null
})

const offerForm = reactive({ price: 0, message: '' })
const submitting = ref(false)
const submitError = ref('')

const showProviderCounter = ref(false)
const counterForm = reactive({ price: 0, message: '' })
const negError = ref('')

async function submitOffer() {
  if (offerForm.price <= 0) return
  submitError.value = ''
  submitting.value = true
  try {
    await $api(`/requests/${requestId}/offers`, {
      method: 'POST',
      body: { price: offerForm.price, message: offerForm.message || null },
    })
    await refresh()
  } catch (e: unknown) {
    submitError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  } finally {
    submitting.value = false
  }
}

async function acceptNegotiation() {
  negError.value = ''
  const n = pendingClientNegotiation.value!
  try {
    await $api(`/requests/${requestId}/offers/${myOffer.value!.id}/negotiations/${n.id}/accept`, { method: 'PUT' })
    await refresh()
  } catch (e: unknown) {
    negError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}

async function declineNegotiation() {
  negError.value = ''
  const n = pendingClientNegotiation.value!
  try {
    await $api(`/requests/${requestId}/offers/${myOffer.value!.id}/negotiations/${n.id}/decline`, { method: 'PUT' })
    await refresh()
  } catch (e: unknown) {
    negError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}

async function submitProviderCounter() {
  negError.value = ''
  const n = pendingClientNegotiation.value!
  try {
    await $api(`/requests/${requestId}/offers/${myOffer.value!.id}/negotiations/${n.id}/counter`, {
      method: 'POST',
      body: { proposedPrice: counterForm.price, message: counterForm.message || null },
    })
    showProviderCounter.value = false
    await refresh()
  } catch (e: unknown) {
    negError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}

function statusLabel(s: ServiceRequestStatus) {
  const map: Record<ServiceRequestStatus, string> = {
    Open: 'Åben', OfferReceived: 'Tilbud modtaget', Accepted: 'Accepteret',
    Declined: 'Afvist', Cancelled: 'Annulleret', Completed: 'Afsluttet',
  }
  return map[s]
}

function statusBadge(s: ServiceRequestStatus) {
  const map: Record<ServiceRequestStatus, string> = {
    Open: 'badge-blue', OfferReceived: 'badge-yellow', Accepted: 'badge-green',
    Declined: 'badge-red', Cancelled: 'badge-gray', Completed: 'badge-purple',
  }
  return map[s]
}

function offerStatusLabel(s: OfferStatus) {
  const map: Record<OfferStatus, string> = {
    Pending: 'Afventer', Accepted: 'Accepteret', Declined: 'Afvist', Countered: 'Modtilbud',
  }
  return map[s]
}

function offerStatusBadge(s: OfferStatus) {
  const map: Record<OfferStatus, string> = {
    Pending: 'badge-yellow', Accepted: 'badge-green', Declined: 'badge-red', Countered: 'badge-blue',
  }
  return map[s]
}

function negStatusLabel(s: NegotiationStatus) {
  const map: Record<NegotiationStatus, string> = { Pending: 'Afventer', Accepted: 'Accepteret', Declined: 'Afvist' }
  return map[s]
}

function negStatusBadge(s: NegotiationStatus) {
  const map: Record<NegotiationStatus, string> = { Pending: 'badge-yellow', Accepted: 'badge-green', Declined: 'badge-red' }
  return map[s]
}
</script>
