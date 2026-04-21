<template>
  <div class="max-w-2xl mx-auto">
    <button @click="$router.back()" class="btn-ghost mb-6 -ml-3">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/></svg>
      Tilbage
    </button>

    <div v-if="pending" class="card flex items-center justify-center py-12">
      <div class="w-6 h-6 border-2 border-violet-600 border-t-transparent rounded-full animate-spin"></div>
    </div>

    <template v-else-if="request">
      <!-- Header -->
      <div class="flex items-center justify-between mb-6">
        <div>
          <h2 class="page-header">Forespørgsel</h2>
          <p class="page-sub">{{ request.categoryName ?? 'Fri tekst forespørgsel' }}</p>
        </div>
        <span :class="statusBadge(request.status)">{{ statusLabel(request.status) }}</span>
      </div>

      <!-- Request details -->
      <div class="card mb-4">
        <p class="section-label">Detaljer</p>
        <div class="space-y-2.5">
          <div class="flex justify-between items-center">
            <span class="text-sm text-gray-500">Kategori</span>
            <span class="font-medium text-gray-900 text-sm">{{ request.categoryName ?? 'Fri tekst' }}</span>
          </div>
          <div v-if="request.freeTextDescription" class="flex justify-between items-start">
            <span class="text-sm text-gray-500">Beskrivelse</span>
            <span class="font-medium text-gray-900 text-sm max-w-xs text-right leading-snug">{{ request.freeTextDescription }}</span>
          </div>
          <div class="divider"></div>
          <div class="grid grid-cols-3 gap-4 text-center">
            <div class="bg-gray-50 rounded-xl p-3">
              <p class="text-xs text-gray-400 mb-1">Dato</p>
              <p class="font-semibold text-sm text-gray-900">{{ request.requestedDate }}</p>
            </div>
            <div class="bg-gray-50 rounded-xl p-3">
              <p class="text-xs text-gray-400 mb-1">Tidspunkt</p>
              <p class="font-semibold text-sm text-gray-900">{{ request.requestedTime.slice(0, 5) }}</p>
            </div>
            <div class="bg-gray-50 rounded-xl p-3">
              <p class="text-xs text-gray-400 mb-1">By</p>
              <p class="font-semibold text-sm text-gray-900">{{ request.city }}</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Existing offer from this provider -->
      <div v-if="myOffer" class="card mb-4">
        <div class="flex items-center justify-between mb-4">
          <p class="section-label mb-0">Dit tilbud</p>
          <span :class="offerStatusBadge(myOffer.status)">{{ offerStatusLabel(myOffer.status) }}</span>
        </div>

        <div class="bg-violet-50 rounded-xl px-5 py-4 mb-4 flex items-baseline gap-2">
          <span class="text-3xl font-bold text-violet-700">{{ myOffer.price.toFixed(0) }}</span>
          <span class="text-sm text-violet-500 font-medium">DKK</span>
        </div>

        <p v-if="myOffer.message" class="text-sm text-gray-600 mb-4 leading-snug">{{ myOffer.message }}</p>

        <!-- Negotiation thread -->
        <div v-if="myOffer.negotiations?.length">
          <p class="section-label">Forhandlingshistorik</p>
          <div class="space-y-2">
            <div v-for="n in myOffer.negotiations" :key="n.id"
                 :class="['rounded-xl px-4 py-3 border', n.initiatedBy === 'Provider' ? 'bg-violet-50 border-violet-100 mr-8' : 'bg-gray-50 border-gray-100 ml-8']">
              <div class="flex items-center justify-between mb-1">
                <span class="text-xs font-semibold" :class="n.initiatedBy === 'Provider' ? 'text-violet-700' : 'text-gray-500'">
                  {{ n.initiatedBy === 'Provider' ? 'Dig' : 'Klient' }}
                </span>
                <div class="flex items-center gap-2">
                  <span class="text-sm font-bold text-gray-900">{{ n.proposedPrice.toFixed(0) }} DKK</span>
                  <span :class="negStatusBadge(n.status)">{{ negStatusLabel(n.status) }}</span>
                </div>
              </div>
              <p v-if="n.message" class="text-xs text-gray-500 leading-snug">{{ n.message }}</p>
            </div>
          </div>
        </div>

        <!-- Respond to pending client counter -->
        <div v-if="pendingClientNegotiation" class="border-t border-gray-100 pt-4 mt-4">
          <div class="bg-amber-50 border border-amber-200 rounded-xl px-4 py-3 mb-4">
            <p class="text-xs font-semibold text-amber-600 uppercase tracking-wide mb-1">Klientens forslag</p>
            <div class="flex items-baseline gap-1.5">
              <span class="text-2xl font-bold text-amber-900">{{ pendingClientNegotiation.proposedPrice.toFixed(0) }}</span>
              <span class="text-sm text-amber-600">DKK</span>
            </div>
            <p v-if="pendingClientNegotiation.message" class="text-sm text-amber-700 mt-1 italic">"{{ pendingClientNegotiation.message }}"</p>
          </div>

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

          <div v-if="showProviderCounter" class="space-y-3 border-t border-gray-100 pt-3 mt-3">
            <div class="relative">
              <input v-model.number="counterForm.price" type="number" min="1" class="input-field pr-14" placeholder="Dit modtilbud" />
              <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm text-gray-400 font-medium">DKK</span>
            </div>
            <textarea v-model="counterForm.message" rows="2" class="input-field" placeholder="Besked (valgfri)" />
            <div class="flex gap-2">
              <button @click="submitProviderCounter" class="btn-primary flex-1">Send modtilbud</button>
              <button @click="showProviderCounter = false" class="btn-secondary px-4">Annuller</button>
            </div>
          </div>
          <button v-else @click="showProviderCounter = true" class="btn-ghost w-full justify-center mt-1 text-xs">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"/></svg>
            Foreslå anden pris
          </button>
        </div>
      </div>

      <!-- Submit new offer -->
      <div v-else-if="request.status === 'Open' || request.status === 'OfferReceived'" class="card">
        <p class="section-label">Afgiv tilbud</p>
        <p class="text-sm text-gray-500 mb-5 leading-snug">Indgiv din pris for at besvare denne forespørgsel. Klienten kan acceptere, afvise eller forhandle.</p>

        <div v-if="submitError" class="alert-error mb-4">
          <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
          {{ submitError }}
        </div>

        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Din pris</label>
            <div class="relative">
              <input v-model.number="offerForm.price" type="number" min="1" class="input-field pr-14 text-lg font-semibold" placeholder="0" />
              <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm text-gray-400 font-medium">DKK</span>
            </div>
          </div>
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Besked til klienten <span class="text-gray-400 font-normal">(valgfri)</span></label>
            <textarea v-model="offerForm.message" rows="3" class="input-field" placeholder="Beskriv hvad din pris inkluderer…" />
          </div>
        </div>

        <button @click="submitOffer" :disabled="submitting || offerForm.price <= 0" class="btn-primary mt-5 w-full py-3">
          <svg v-if="submitting" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24"><circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/><path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/></svg>
          <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8"/></svg>
          {{ submitting ? 'Sender…' : 'Send tilbud' }}
        </button>
      </div>

      <!-- Already accepted or closed -->
      <div v-else-if="request.status === 'Accepted'" class="card text-center py-8">
        <div class="w-12 h-12 rounded-2xl bg-emerald-100 flex items-center justify-center mx-auto mb-3">
          <svg class="w-6 h-6 text-emerald-600" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
        </div>
        <p class="font-semibold text-gray-800">Booking bekræftet</p>
        <p class="text-sm text-gray-500 mt-1">Denne forespørgsel er accepteret og er nu en booking.</p>
      </div>
    </template>

    <div v-else class="card text-center py-12">
      <p class="font-semibold text-gray-700">Forespørgsel ikke fundet.</p>
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
