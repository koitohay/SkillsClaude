<template>
  <div class="max-w-3xl mx-auto">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h2 class="text-2xl font-bold text-gray-900">Indgående forespørgsler</h2>
        <p class="text-sm text-gray-500 mt-1">Forespørgsler der matcher dine kategorier</p>
      </div>
    </div>

    <!-- Bookings banner -->
    <div v-if="acceptedCount > 0" class="flex items-center gap-3 bg-emerald-50 border border-emerald-200 rounded-2xl px-5 py-3 mb-6">
      <svg class="w-5 h-5 text-emerald-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
      <p class="text-sm text-emerald-800 font-medium">Du har {{ acceptedCount }} bekræftet booking{{ acceptedCount > 1 ? 'er' : '' }}.</p>
      <NuxtLink to="/provider/bookings" class="ml-auto text-sm font-semibold text-emerald-700 hover:text-emerald-900">Se bookinger →</NuxtLink>
    </div>

    <div v-if="pending" class="space-y-4">
      <div v-for="i in 4" :key="i" class="card animate-pulse">
        <div class="h-5 bg-gray-200 rounded w-1/3 mb-3"></div>
        <div class="h-4 bg-gray-100 rounded w-2/3"></div>
      </div>
    </div>

    <div v-else-if="!activeRequests.length" class="card text-center py-16">
      <div class="w-16 h-16 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
        <svg class="w-8 h-8 text-violet-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M20 13V6a2 2 0 00-2-2H6a2 2 0 00-2 2v7m16 0v5a2 2 0 01-2 2H6a2 2 0 01-2-2v-5m16 0h-2.586a1 1 0 00-.707.293l-2.414 2.414a1 1 0 01-.707.293h-3.172a1 1 0 01-.707-.293l-2.414-2.414A1 1 0 006.586 13H4"/></svg>
      </div>
      <p class="font-semibold text-gray-700 mb-2">Ingen relevante forespørgsler</p>
      <p class="text-sm text-gray-500">Du vil se forespørgsler der matcher dine servicekategorier her.</p>
    </div>

    <div v-else class="space-y-4">
      <div v-for="req in activeRequests" :key="req.id" class="card hover:shadow-md hover:border-violet-200 transition-all">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-1.5 flex-wrap">
              <span class="font-semibold text-gray-900">{{ req.categoryName ?? 'Fri tekst' }}</span>
              <span :class="statusBadge(req.status)">{{ statusLabel(req.status) }}</span>
              <!-- Badge when this provider has already submitted an offer -->
              <span v-if="hasMyOffer(req)" class="badge-purple">Tilbud afsendt</span>
            </div>
            <p v-if="req.freeTextDescription" class="text-sm text-gray-600 mb-2 leading-snug line-clamp-2">
              {{ req.freeTextDescription }}
            </p>
            <p class="text-sm text-gray-500">
              {{ req.requestedDate }} · {{ req.requestedTime.slice(0, 5) }} · {{ req.city }}
            </p>
          </div>
          <NuxtLink :to="`/provider/requests/${req.id}`"
                    :class="hasMyOffer(req) ? 'btn-secondary' : 'btn-primary'"
                    class="text-sm whitespace-nowrap flex-shrink-0">
            {{ hasMyOffer(req) ? 'Se detaljer' : 'Afgiv tilbud' }}
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'provider-only' })

import type { ServiceRequestDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const authStore = useAuthStore()
const { requestStatusLabel: statusLabel, requestStatusBadge: statusBadge } = useStatusHelpers()

const { data: requests, pending } = await useAsyncData('provider-requests', () =>
  $api<ServiceRequestDto[]>('/providers/me/requests'))

const activeRequests = computed(() =>
  (requests.value ?? []).filter(r => r.status !== 'Accepted'))

const acceptedCount = computed(() =>
  (requests.value ?? []).filter(r => r.status === 'Accepted').length)

function hasMyOffer(req: ServiceRequestDto): boolean {
  return req.offers?.some(o => o.serviceProviderId === authStore.userId) ?? false
}
</script>
