<template>
  <div class="max-w-3xl mx-auto">
    <div class="flex items-center justify-between mb-6">
      <div>
        <h2 class="text-2xl font-bold text-gray-900">Mine forespørgsler</h2>
        <p class="text-sm text-gray-500 mt-1">Åbne og afventende forespørgsler</p>
      </div>
      <NuxtLink to="/booking/new" class="btn-primary">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
        Ny forespørgsel
      </NuxtLink>
    </div>

    <!-- Bookings banner -->
    <div v-if="acceptedCount > 0" class="flex items-center gap-3 bg-emerald-50 border border-emerald-200 rounded-2xl px-5 py-3 mb-6">
      <svg class="w-5 h-5 text-emerald-500 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
      <p class="text-sm text-emerald-800 font-medium">Du har {{ acceptedCount }} bekræftet booking{{ acceptedCount > 1 ? 'er' : '' }}.</p>
      <NuxtLink to="/bookings" class="ml-auto text-sm font-semibold text-emerald-700 hover:text-emerald-900">Se bookinger →</NuxtLink>
    </div>

    <div v-if="pending" class="space-y-4">
      <div v-for="i in 3" :key="i" class="card animate-pulse">
        <div class="h-5 bg-gray-200 rounded w-1/3 mb-3"></div>
        <div class="h-4 bg-gray-100 rounded w-2/3"></div>
      </div>
    </div>

    <div v-else-if="!activeRequests.length" class="card text-center py-16">
      <div class="w-16 h-16 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
        <svg class="w-8 h-8 text-violet-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2"/></svg>
      </div>
      <p class="font-semibold text-gray-700 mb-2">Ingen aktive forespørgsler</p>
      <p class="text-sm text-gray-500 mb-6">Opret en forespørgsel for at modtage tilbud fra udbydere.</p>
      <NuxtLink to="/" class="btn-primary inline-flex">Find en service</NuxtLink>
    </div>

    <div v-else class="space-y-4">
      <div v-for="req in activeRequests" :key="req.id" class="card hover:shadow-md hover:border-violet-200 transition-all">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-1.5 flex-wrap">
              <span class="font-semibold text-gray-900">{{ req.categoryName ?? 'Fri tekst' }}</span>
              <span :class="statusBadge(req.status)">{{ statusLabel(req.status) }}</span>
            </div>
            <p v-if="req.freeTextDescription" class="text-sm text-gray-600 mb-2 leading-snug line-clamp-2">
              {{ req.freeTextDescription }}
            </p>
            <p class="text-sm text-gray-500">
              {{ req.requestedDate }} · {{ req.requestedTime.slice(0, 5) }} · {{ req.city }}
            </p>
          </div>
          <NuxtLink :to="`/booking/${req.id}/offers`" class="btn-secondary text-sm whitespace-nowrap flex-shrink-0">
            Se tilbud
            <span v-if="pendingOfferCount(req) > 0"
                  class="ml-1.5 bg-violet-600 text-white text-xs rounded-full px-1.5 py-0.5 font-bold">
              {{ pendingOfferCount(req) }}
            </span>
          </NuxtLink>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'client-only' })

import type { ServiceRequestDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const { requestStatusLabel: statusLabel, requestStatusBadge: statusBadge } = useStatusHelpers()

const { data: requests, pending } = await useAsyncData('my-requests', () =>
  $api<ServiceRequestDto[]>('/requests'))

const activeRequests = computed(() =>
  (requests.value ?? []).filter(r => r.status !== 'Accepted'))

const acceptedCount = computed(() =>
  (requests.value ?? []).filter(r => r.status === 'Accepted').length)

function pendingOfferCount(req: ServiceRequestDto) {
  return req.offers?.filter(o => o.status === 'Pending' || o.status === 'Countered').length ?? 0
}
</script>
