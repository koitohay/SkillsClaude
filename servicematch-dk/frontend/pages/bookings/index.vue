<template>
  <div class="max-w-3xl mx-auto">
    <div class="flex items-center justify-between mb-8">
      <div>
        <h2 class="page-header">Mine bookinger</h2>
        <p class="page-sub">Accepterede aftaler med udbydere</p>
      </div>
      <NuxtLink to="/booking/new" class="btn-primary">
        <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/></svg>
        Ny forespørgsel
      </NuxtLink>
    </div>

    <div v-if="pending" class="space-y-4">
      <div v-for="i in 3" :key="i" class="card animate-pulse">
        <div class="h-5 bg-gray-200 rounded w-1/3 mb-3"></div>
        <div class="h-4 bg-gray-100 rounded w-2/3"></div>
      </div>
    </div>

    <div v-else-if="!bookings.length" class="card text-center py-16">
      <div class="w-16 h-16 rounded-2xl bg-violet-100 flex items-center justify-center mx-auto mb-4">
        <svg class="w-8 h-8 text-violet-400" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
      </div>
      <h3 class="font-semibold text-gray-700 mb-2">Ingen bookinger endnu</h3>
      <p class="text-sm text-gray-500 mb-6">Når du accepterer et tilbud, vises det her som en bekræftet booking.</p>
      <NuxtLink to="/" class="btn-primary inline-flex">Find en service</NuxtLink>
    </div>

    <div v-else class="space-y-4">
      <div v-for="booking in bookings" :key="booking.id" class="card hover:shadow-md hover:border-violet-200 transition-all">
        <div class="flex items-start justify-between gap-4">
          <div class="flex-1 min-w-0">
            <div class="flex items-center gap-2 mb-2">
              <span class="font-bold text-gray-900">{{ booking.categoryName ?? 'Fri tekst' }}</span>
              <span class="badge-green">Bekræftet</span>
            </div>
            <p v-if="booking.freeTextDescription" class="text-sm text-gray-600 mb-2 leading-snug">
              {{ booking.freeTextDescription }}
            </p>
            <div class="flex flex-wrap items-center gap-x-4 gap-y-1 text-sm text-gray-500">
              <span class="flex items-center gap-1">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/></svg>
                {{ booking.requestedDate }}
              </span>
              <span class="flex items-center gap-1">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0"/></svg>
                {{ booking.requestedTime.slice(0, 5) }}
              </span>
              <span class="flex items-center gap-1">
                <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
                {{ booking.city }}
              </span>
            </div>
          </div>
          <div class="text-right flex-shrink-0">
            <div v-if="acceptedPrice(booking)" class="mb-3">
              <p class="text-xs text-gray-400">Aftalt pris</p>
              <p class="text-xl font-bold text-violet-600">{{ acceptedPrice(booking) }} <span class="text-sm font-normal text-gray-500">DKK</span></p>
            </div>
            <NuxtLink :to="`/booking/${booking.id}/offers`" class="btn-ghost text-xs">
              Se detaljer →
            </NuxtLink>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
definePageMeta({ middleware: 'auth' })

import type { ServiceRequestDto } from '~/types/api.types'

const { $api } = useNuxtApp()

const { data: allRequests, pending } = await useAsyncData('my-requests-bookings', () =>
  $api<ServiceRequestDto[]>('/requests'))

const bookings = computed(() =>
  (allRequests.value ?? []).filter(r => r.status === 'Accepted'))

function acceptedPrice(req: ServiceRequestDto): string | null {
  const offer = req.offers?.find(o => o.status === 'Accepted')
  return offer ? offer.price.toFixed(2) : null
}
</script>
