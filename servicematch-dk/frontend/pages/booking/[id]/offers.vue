<template>
  <div class="max-w-2xl mx-auto">
    <button @click="$router.back()" class="btn-ghost mb-6 -ml-3">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/></svg>
      Tilbage
    </button>

    <div class="flex items-center justify-between mb-6">
      <div>
        <h2 class="page-header">Tilbud på din forespørgsel</h2>
        <p class="page-sub">Gennemse og accepter tilbud fra udbydere</p>
      </div>
    </div>

    <div v-if="pending" class="space-y-4">
      <div v-for="i in 3" :key="i" class="card animate-pulse">
        <div class="h-6 bg-gray-200 rounded w-1/4 mb-3"></div>
        <div class="h-4 bg-gray-100 rounded w-2/3"></div>
      </div>
    </div>

    <div v-else-if="!offers?.length" class="card text-center py-16">
      <div class="w-16 h-16 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
        <svg class="w-8 h-8 text-violet-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
      </div>
      <h3 class="font-semibold text-gray-700 mb-2">Ingen tilbud endnu</h3>
      <p class="text-sm text-gray-500">Udbydere kan se din forespørgsel og vil sende tilbud snart.</p>
    </div>

    <div v-if="actionError" class="alert-error mb-4">
      <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
      {{ actionError }}
    </div>

    <div v-else class="space-y-4">
      <div v-for="offer in offers" :key="offer.id"
           :class="['card border-l-4 transition-all', offerBorderClass(offer.status)]">
        <!-- Offer header -->
        <div class="flex items-start justify-between gap-4 mb-3">
          <div>
            <div class="flex items-center gap-2 mb-1">
              <span class="text-2xl font-bold text-gray-900">{{ offer.price.toFixed(0) }}</span>
              <span class="text-sm text-gray-500 font-medium">DKK</span>
              <span :class="statusClass(offer.status)">{{ statusLabel(offer.status) }}</span>
            </div>
            <p v-if="offer.message" class="text-sm text-gray-600 leading-snug">{{ offer.message }}</p>
          </div>

          <!-- Action buttons for actionable offers -->
          <div v-if="offer.status === 'Pending' || offer.status === 'Countered'" class="flex flex-col gap-2 flex-shrink-0">
            <button @click="acceptOffer(offer.id)" :disabled="actionLoading === offer.id" class="btn-primary text-xs px-4 py-2">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/></svg>
              Accepter
            </button>
            <button @click="declineOffer(offer.id)" :disabled="actionLoading === offer.id + '-decline'" class="btn-danger text-xs px-4 py-2">
              <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/></svg>
              Afvis
            </button>
          </div>
        </div>

        <!-- Negotiation thread -->
        <div v-if="offer.negotiations?.length" class="border-t border-gray-100 pt-3 mt-3 space-y-2">
          <p class="section-label">Forhandlingshistorik</p>
          <div v-for="n in offer.negotiations" :key="n.id"
               :class="['rounded-xl px-4 py-2.5 border', n.initiatedBy === 'Client' ? 'bg-violet-50 border-violet-100 ml-6' : 'bg-gray-50 border-gray-100 mr-6']">
            <div class="flex items-center justify-between mb-0.5">
              <span class="text-xs font-semibold" :class="n.initiatedBy === 'Client' ? 'text-violet-700' : 'text-gray-600'">
                {{ n.initiatedBy === 'Client' ? 'Dig' : 'Udbyder' }}
              </span>
              <span class="text-sm font-bold text-gray-900">{{ n.proposedPrice.toFixed(0) }} DKK</span>
            </div>
            <p v-if="n.message" class="text-xs text-gray-500">{{ n.message }}</p>
          </div>
        </div>

        <!-- Counter offer form (inline) -->
        <div v-if="(offer.status === 'Pending' || offer.status === 'Countered') && counteringOfferId === offer.id"
             class="border-t border-gray-100 pt-4 mt-3">
          <p class="section-label">Send modtilbud</p>
          <div class="space-y-3">
            <div class="relative">
              <input v-model.number="counterForm.price" type="number" min="1" class="input-field pr-14" placeholder="Din pris" />
              <span class="absolute right-4 top-1/2 -translate-y-1/2 text-sm text-gray-400 font-medium">DKK</span>
            </div>
            <textarea v-model="counterForm.message" rows="2" class="input-field" placeholder="Besked til udbyder (valgfri)" />
            <div class="flex gap-2">
              <button @click="submitCounter(offer.id)" class="btn-primary flex-1">Send modtilbud</button>
              <button @click="counteringOfferId = null" class="btn-secondary px-4">Annuller</button>
            </div>
          </div>
        </div>

        <!-- Toggle counter form -->
        <div v-else-if="offer.status === 'Pending' || offer.status === 'Countered'" class="border-t border-gray-100 pt-3 mt-3">
          <button @click="openCounter(offer)" class="btn-ghost text-xs">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7h12m0 0l-4-4m4 4l-4 4m0 6H4m0 0l4 4m-4-4l4-4"/></svg>
            Foreslå anden pris
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'client-only' })

import type { OfferDto, OfferStatus } from '~/types/api.types'

const { $api } = useNuxtApp()
const route = useRoute()
const requestId = route.params.id as string
const { offerStatusLabel: statusLabel, offerStatusBadge: statusClass } = useStatusHelpers()

const { data: offers, pending, refresh } = await useAsyncData(`offers-${requestId}`, () =>
  $api<OfferDto[]>(`/requests/${requestId}/offers`))

const counteringOfferId = ref<string | null>(null)
const counterForm = reactive({ price: 0, message: '' })
const actionError = ref('')
const actionLoading = ref<string | null>(null)
const counterError = ref('')

function openCounter(offer: OfferDto) {
  counteringOfferId.value = offer.id
  counterForm.price = offer.price
  counterForm.message = ''
}

async function acceptOffer(offerId: string) {
  actionError.value = ''
  actionLoading.value = offerId
  try {
    await $api(`/requests/${requestId}/offers/${offerId}/accept`, { method: 'PUT' })
    await navigateTo('/bookings')
  } catch (e: unknown) {
    actionError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
    await refresh()
  } finally {
    actionLoading.value = null
  }
}

async function declineOffer(offerId: string) {
  actionError.value = ''
  actionLoading.value = offerId + '-decline'
  try {
    await $api(`/requests/${requestId}/offers/${offerId}/decline`, { method: 'PUT' })
    await refresh()
  } catch (e: unknown) {
    actionError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  } finally {
    actionLoading.value = null
  }
}

async function submitCounter(offerId: string) {
  counterError.value = ''
  try {
    await $api(`/requests/${requestId}/offers/${offerId}/counter`, {
      method: 'POST',
      body: { proposedPrice: counterForm.price, message: counterForm.message || null },
    })
    counteringOfferId.value = null
    await refresh()
  } catch (e: unknown) {
    counterError.value = (e as { data?: { error?: string } })?.data?.error ?? 'Noget gik galt.'
  }
}

function offerBorderClass(s: OfferStatus) {
  const map: Record<OfferStatus, string> = {
    Pending: 'border-l-amber-400', Accepted: 'border-l-emerald-400',
    Declined: 'border-l-red-300', Countered: 'border-l-blue-400'
  }
  return map[s]
}
</script>
