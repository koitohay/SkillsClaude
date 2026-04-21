<template>
  <div>
    <!-- Search bar -->
    <div class="flex gap-3 mb-8">
      <input
        v-model="searchInput"
        @keydown.enter="doSearch"
        class="input-field flex-1 text-base"
        placeholder="Søg på ydelse eller udbyder…"
        autofocus
      />
      <button @click="doSearch" class="btn-primary px-6">Søg</button>
    </div>

    <div v-if="!q" class="card text-center py-16">
      <div class="text-4xl mb-3">🔍</div>
      <p class="font-semibold text-gray-700">Skriv hvad du søger ovenfor</p>
      <p class="text-sm text-gray-500 mt-1">F.eks. "tandlæge", "massage" eller "klipning"</p>
    </div>

    <template v-else>
      <div class="flex items-center justify-between mb-5">
        <div>
          <h1 class="text-xl font-bold text-gray-900">Resultater for "{{ q }}"</h1>
          <p class="text-sm text-gray-500 mt-0.5">{{ providers?.length ?? 0 }} udbydere fundet</p>
        </div>
      </div>

      <div v-if="pending" class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div v-for="i in 4" :key="i" class="card animate-pulse">
          <div class="h-5 bg-gray-200 rounded-lg w-2/3 mb-3"></div>
          <div class="h-4 bg-gray-100 rounded-lg w-1/3 mb-6"></div>
          <div class="space-y-3">
            <div class="h-16 bg-gray-100 rounded-xl"></div>
            <div class="h-16 bg-gray-100 rounded-xl"></div>
          </div>
        </div>
      </div>

      <div v-else-if="!providers?.length" class="card text-center py-16">
        <div class="text-4xl mb-4">😕</div>
        <h3 class="font-semibold text-gray-700 mb-2">Ingen resultater</h3>
        <p class="text-sm text-gray-500">Prøv et andet søgeord eller browse en kategori.</p>
        <NuxtLink to="/" class="btn-secondary mt-6 inline-flex">Gå til forside</NuxtLink>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div v-for="provider in providers" :key="provider.id" class="card hover:shadow-md hover:border-violet-200 transition-all flex flex-col">
          <div class="flex items-start justify-between mb-4">
            <div>
              <h3 class="font-bold text-gray-900 text-lg">{{ provider.companyName }}</h3>
              <div class="flex items-center gap-2 mt-1 text-sm text-gray-500">
                <svg class="w-3.5 h-3.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
                {{ provider.city }}
                <span class="text-gray-300">·</span>
                <svg class="w-3.5 h-3.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/></svg>
                {{ provider.contactName }}
              </div>
            </div>
            <div v-if="minPrice(provider)" class="text-right flex-shrink-0 ml-4">
              <p class="text-xs text-gray-400">Fra</p>
              <p class="font-bold text-violet-600 text-lg">{{ minPrice(provider) }} <span class="text-sm font-normal">DKK</span></p>
            </div>
          </div>

          <div class="flex-1 space-y-2 mb-4">
            <p class="text-xs font-semibold text-gray-400 uppercase tracking-wide mb-3">Ydelser</p>
            <div v-if="!provider.services.length" class="text-sm text-gray-400 italic">Ingen ydelser listet</div>
            <div
              v-for="service in provider.services.slice(0, showAll[provider.id] ? undefined : 3)"
              :key="service.id"
              class="flex items-start justify-between bg-gray-50 rounded-xl px-4 py-3 hover:bg-violet-50 hover:border-violet-200 border border-transparent transition-all cursor-pointer group"
              @click="selectService(provider, service)"
            >
              <div class="flex-1 min-w-0 mr-3">
                <p class="font-semibold text-gray-800 text-sm group-hover:text-violet-700">{{ service.name }}</p>
                <p class="text-xs text-gray-500 mt-0.5 leading-relaxed">{{ service.description }}</p>
              </div>
              <div class="text-right flex-shrink-0">
                <p class="font-bold text-gray-900 text-sm">{{ service.basePrice.toFixed(0) }} DKK</p>
                <p class="text-xs text-violet-500 font-medium opacity-0 group-hover:opacity-100 transition-opacity mt-0.5">Vælg →</p>
              </div>
            </div>
            <button
              v-if="provider.services.length > 3"
              @click="showAll[provider.id] = !showAll[provider.id]"
              class="text-xs text-violet-600 font-semibold hover:text-violet-700 px-1 pt-1"
            >
              {{ showAll[provider.id] ? '↑ Vis færre' : `+ ${provider.services.length - 3} mere ydelser` }}
            </button>
          </div>

          <button @click="requestQuote(provider)" class="btn-secondary w-full mt-auto">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
            Spørg om pris hos {{ provider.companyName }}
          </button>
        </div>
      </div>
    </template>
  </div>
</template>

<script setup lang="ts">
import type { ProviderWithServicesDto, ProviderServiceListingDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const route = useRoute()
const router = useRouter()
const bookingStore = useBookingStore()

const q = computed(() => (route.query.q as string) || '')
const searchInput = ref(q.value)
const showAll = reactive<Record<string, boolean>>({})

const { data: providers, pending, refresh } = await useAsyncData(
  `search-${q.value}`,
  () => q.value ? $api<ProviderWithServicesDto[]>(`/providers?searchTerm=${encodeURIComponent(q.value)}`) : Promise.resolve([]),
)

watch(q, () => refresh())

function doSearch() {
  if (searchInput.value.trim())
    router.push(`/search?q=${encodeURIComponent(searchInput.value.trim())}`)
}

function minPrice(provider: ProviderWithServicesDto): string | null {
  if (!provider.services.length) return null
  return Math.min(...provider.services.map(s => s.basePrice)).toFixed(0)
}

function selectService(provider: ProviderWithServicesDto, service: ProviderServiceListingDto) {
  bookingStore.wizard.categoryId = service.categoryId ?? null
  bookingStore.wizard.selectedServiceName = service.name
  bookingStore.wizard.freeText = `${service.name} hos ${provider.companyName}: ${service.description}`
  router.push('/booking/schedule')
}

function requestQuote(provider: ProviderWithServicesDto) {
  bookingStore.wizard.selectedServiceName = ''
  bookingStore.wizard.freeText = `Forespørgsel hos ${provider.companyName}`
  router.push('/booking/schedule')
}
</script>
