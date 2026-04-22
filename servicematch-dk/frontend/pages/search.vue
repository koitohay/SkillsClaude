<template>
  <div>
    <!-- Search bar -->
    <div class="flex gap-3 mb-8">
      <div class="relative flex-1">
        <svg class="absolute left-4 top-1/2 -translate-y-1/2 w-4 h-4 pointer-events-none" style="color: var(--text-3);" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"/></svg>
        <input
          v-model="searchInput"
          @keydown.enter="doSearch"
          class="input-field pl-11"
          placeholder="Søg på ydelse eller udbyder…"
          autofocus
        />
      </div>
      <button @click="doSearch" class="btn-primary px-6">Søg</button>
    </div>

    <!-- Prompt state -->
    <div v-if="!q" class="card text-center py-16">
      <div class="w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4"
           style="background: rgb(124 58 237 / 0.06);">
        <svg class="w-7 h-7" style="color: rgb(124 58 237 / 0.35);" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0"/></svg>
      </div>
      <p class="font-semibold mb-1" style="color: var(--text-1);">Skriv hvad du søger ovenfor</p>
      <p class="text-sm" style="color: var(--text-2);">F.eks. "tandlæge", "massage" eller "klipning"</p>
    </div>

    <template v-else>
      <div class="flex items-center justify-between mb-6">
        <div>
          <h1 class="text-xl font-bold" style="color: var(--text-1); letter-spacing: -0.025em;">Resultater for "{{ q }}"</h1>
          <p class="text-sm mt-0.5" style="color: var(--text-2);">{{ providers?.length ?? 0 }} udbydere fundet</p>
        </div>
      </div>

      <!-- Skeletons -->
      <div v-if="pending" class="grid grid-cols-1 md:grid-cols-2 gap-5">
        <div v-for="i in 4" :key="i" class="card">
          <div class="h-5 rounded skeleton w-2/3 mb-3"></div>
          <div class="h-3.5 rounded skeleton w-1/3 mb-6"></div>
          <div class="space-y-3">
            <div class="h-16 rounded-xl skeleton"></div>
            <div class="h-16 rounded-xl skeleton"></div>
          </div>
        </div>
      </div>

      <!-- No results -->
      <div v-else-if="!providers?.length" class="card text-center py-16">
        <div class="w-16 h-16 rounded-2xl flex items-center justify-center mx-auto mb-4"
             style="background: rgb(156 163 175 / 0.1);">
          <svg class="w-7 h-7" style="color: rgb(156 163 175 / 0.5);" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="1.5" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/></svg>
        </div>
        <h3 class="font-semibold mb-2" style="color: var(--text-1);">Ingen resultater</h3>
        <p class="text-sm mb-6" style="color: var(--text-2);">Prøv et andet søgeord eller browse en kategori.</p>
        <NuxtLink to="/" class="btn-secondary">Gå til forside</NuxtLink>
      </div>

      <!-- Provider grid -->
      <div v-else class="grid grid-cols-1 md:grid-cols-2 gap-5">
        <div v-for="provider in providers" :key="provider.id"
             class="card flex flex-col transition-all duration-200"
             style="padding: 1.5rem;"
             onmouseenter="this.style.borderColor='#c4b5fd'; this.style.boxShadow='var(--shadow-md), 0 0 0 3px rgb(124 58 237 / 0.04)'; this.style.transform='translateY(-2px)'"
             onmouseleave="this.style.borderColor=''; this.style.boxShadow=''; this.style.transform=''">

          <!-- Provider header -->
          <div class="flex items-start justify-between mb-4">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-xl flex items-center justify-center flex-shrink-0 font-bold text-sm text-white"
                   style="background: linear-gradient(135deg, #7c3aed 0%, #5b21b6 100%);">
                {{ provider.companyName.charAt(0).toUpperCase() }}
              </div>
              <div>
                <h3 class="font-bold" style="color: var(--text-1);">{{ provider.companyName }}</h3>
                <div class="flex items-center gap-1.5 mt-0.5 text-xs" style="color: var(--text-3);">
                  <svg class="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/></svg>
                  {{ provider.city }}
                  <span>·</span>
                  {{ provider.contactName }}
                </div>
              </div>
            </div>
            <div v-if="minPrice(provider)" class="text-right flex-shrink-0 ml-3">
              <p class="text-xs mb-0.5" style="color: var(--text-3);">Fra</p>
              <p class="font-bold text-lg leading-none" style="color: var(--violet-lt); letter-spacing: -0.03em;">
                {{ minPrice(provider) }}<span class="text-xs font-normal ml-0.5" style="color: var(--text-3);">DKK</span>
              </p>
            </div>
          </div>

          <!-- Services -->
          <div class="flex-1 mb-4">
            <p class="section-label mb-2">Ydelser</p>
            <div v-if="!provider.services.length" class="text-sm italic" style="color: var(--text-3);">Ingen ydelser listet</div>
            <div class="space-y-1.5">
              <div
                v-for="service in provider.services.slice(0, showAll[provider.id] ? undefined : 3)"
                :key="service.id"
                class="service-row group"
                @click="selectService(provider, service)"
              >
                <div class="flex-1 min-w-0 mr-3">
                  <p class="font-semibold text-sm transition-colors duration-150 group-hover:text-violet-700" style="color: var(--text-1);">{{ service.name }}</p>
                  <p class="text-xs mt-0.5 leading-relaxed" style="color: var(--text-2);">{{ service.description }}</p>
                </div>
                <div class="text-right flex-shrink-0">
                  <p class="font-bold text-sm" style="color: var(--text-1);">{{ service.basePrice.toFixed(0) }} <span class="font-normal text-xs" style="color: var(--text-3);">DKK</span></p>
                  <p class="text-xs font-semibold mt-0.5 opacity-0 group-hover:opacity-100 transition-opacity duration-150" style="color: var(--violet-lt);">Vælg →</p>
                </div>
              </div>
            </div>
            <button
              v-if="provider.services.length > 3"
              @click="showAll[provider.id] = !showAll[provider.id]"
              class="text-xs font-semibold mt-2 px-1 transition-colors duration-150"
              style="color: var(--violet-lt);"
              onmouseenter="this.style.color='var(--violet-dk)'" onmouseleave="this.style.color='var(--violet-lt)'"
            >
              {{ showAll[provider.id] ? '↑ Vis færre' : `+ ${provider.services.length - 3} flere ydelser` }}
            </button>
          </div>

          <div class="flex gap-2 mt-auto">
            <button @click="requestQuote(provider)" class="btn-secondary flex-1">
              <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z"/></svg>
              Spørg om pris
            </button>
          </div>
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
