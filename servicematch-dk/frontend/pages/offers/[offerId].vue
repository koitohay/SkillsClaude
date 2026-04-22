<template>
  <div class="max-w-2xl mx-auto">

    <button @click="$router.back()" class="btn-ghost mb-6 -ml-2">
      <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
      </svg>
      Tilbage
    </button>

    <!-- Loading skeleton -->
    <div v-if="!offer" class="space-y-4">
      <div class="card">
        <div class="h-6 skeleton rounded w-1/3 mb-3"></div>
        <div class="h-4 skeleton rounded w-2/3 mb-2"></div>
        <div class="h-4 skeleton rounded w-1/2"></div>
      </div>
    </div>

    <template v-else>

      <!-- ── Offer hero card ──────────────────────────────────── -->
      <div class="card mb-5" style="padding: 2rem;">

        <!-- Category + city -->
        <div class="flex items-center justify-between mb-6">
          <span class="badge-violet text-sm font-semibold">{{ offer.categoryName }}</span>
          <span class="flex items-center gap-1.5 text-sm font-medium" style="color: var(--text-3);">
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z"/>
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z"/>
            </svg>
            {{ offer.city }}
          </span>
        </div>

        <!-- Price -->
        <div class="rounded-2xl px-8 py-6 mb-6 text-center"
             style="background: linear-gradient(135deg, rgb(237 233 254 / 0.5) 0%, rgb(221 214 254 / 0.25) 100%); border: 1.5px solid #c4b5fd;">
          <p class="text-xs font-semibold uppercase tracking-widest mb-2" style="color: var(--violet-lt);">Tilbudspris</p>
          <p class="text-5xl font-bold leading-none" style="color: var(--text-1); letter-spacing: -0.04em;">
            {{ offer.price.toLocaleString('da-DK') }}
            <span class="text-xl font-medium ml-1" style="color: var(--text-3);">DKK</span>
          </p>
        </div>

        <!-- Provider message -->
        <div v-if="offer.providerMessage" class="mb-6">
          <p class="text-xs font-semibold uppercase tracking-wide mb-2" style="color: var(--text-3);">Udbyderens besked</p>
          <p class="text-sm leading-relaxed" style="color: var(--text-2);">{{ offer.providerMessage }}</p>
        </div>

        <!-- AI curation reason -->
        <div class="rounded-xl px-4 py-3 mb-6"
             style="background: rgb(124 58 237 / 0.04); border: 1.5px solid rgb(196 181 253 / 0.4);">
          <p class="text-xs leading-relaxed" style="color: var(--text-2);">
            <span class="font-bold" style="color: var(--violet-lt);">✦ Udvalgt af AI: </span>
            {{ offer.curationReason }}
          </p>
        </div>

        <!-- Meta row -->
        <div class="flex items-center gap-4 text-xs pb-6 mb-6" style="color: var(--text-3); border-bottom: 1.5px solid var(--border);">
          <span class="flex items-center gap-1.5">
            <svg class="w-3.5 h-3.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0"/>
            </svg>
            Oprettet {{ timeAgo(offer.offerCreatedAt) }}
          </span>
          <span class="w-1 h-1 rounded-full bg-gray-300"></span>
          <span>Tilbud åbent</span>
        </div>

        <!-- ── CTA: logged-in client ── -->
        <template v-if="authStore.isClient">
          <div v-if="bookError" class="alert-error mb-4">
            <svg class="w-4 h-4 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                    d="M12 9v2m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0"/>
            </svg>
            {{ bookError }}
          </div>

          <!-- Step: pick date + time + city -->
          <div v-if="!booked" class="space-y-4">
            <p class="text-sm font-semibold mb-1" style="color: var(--text-1);">Vælg tidspunkt</p>

            <div class="grid grid-cols-2 gap-3">
              <div>
                <label class="block text-xs font-medium mb-1.5" style="color: var(--text-2);">Dato</label>
                <input v-model="bookForm.date" type="date" :min="minDate" class="input-field" />
              </div>
              <div>
                <label class="block text-xs font-medium mb-1.5" style="color: var(--text-2);">Tidspunkt</label>
                <input v-model="bookForm.time" type="time" class="input-field" />
              </div>
            </div>

            <div>
              <label class="block text-xs font-medium mb-1.5" style="color: var(--text-2);">By</label>
              <select v-model="bookForm.city" class="input-field">
                <option value="" disabled>Vælg din by</option>
                <option v-for="city in danishCities" :key="city" :value="city">{{ city }}</option>
              </select>
            </div>

            <button
              @click="bookNow"
              :disabled="booking || !bookForm.date || !bookForm.time || !bookForm.city"
              class="btn-primary w-full py-3 mt-2"
            >
              <svg v-if="booking" class="w-4 h-4 animate-spin" fill="none" viewBox="0 0 24 24">
                <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"/>
                <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"/>
              </svg>
              <svg v-else class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/>
              </svg>
              {{ booking ? 'Opretter forespørgsel…' : 'Book dette tilbud' }}
            </button>
          </div>

          <!-- Success state -->
          <div v-else class="text-center py-4">
            <div class="w-14 h-14 rounded-2xl flex items-center justify-center mx-auto mb-4"
                 style="background: #d1fae5;">
              <svg class="w-7 h-7" style="color: #059669;" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2"
                      d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0"/>
              </svg>
            </div>
            <p class="font-semibold mb-1" style="color: var(--text-1);">Forespørgsel sendt!</p>
            <p class="text-sm mb-4" style="color: var(--text-2);">
              Udbyderen er blevet notificeret. Du kan nu se og acceptere tilbuddet.
            </p>
            <NuxtLink :to="`/booking/${createdRequestId}/offers`" class="btn-primary">
              Se tilbud →
            </NuxtLink>
          </div>
        </template>

        <!-- ── CTA: provider (can't book own offer) ── -->
        <div v-else-if="authStore.isProvider" class="rounded-xl px-5 py-4 text-center"
             style="background: var(--bg); border: 1.5px solid var(--border);">
          <p class="text-sm font-medium" style="color: var(--text-2);">
            Du er logget ind som udbyder. Log ind som klient for at booke dette tilbud.
          </p>
        </div>

        <!-- ── CTA: guest ── -->
        <div v-else class="space-y-3">
          <p class="text-sm text-center mb-4" style="color: var(--text-2);">
            Opret en konto eller log ind for at booke dette tilbud.
          </p>
          <NuxtLink
            :to="`/auth/register?redirect=/offers/${offerId}`"
            class="btn-primary w-full py-3 justify-center"
          >
            Opret konto og book
          </NuxtLink>
          <NuxtLink
            :to="`/auth/login?redirect=/offers/${offerId}`"
            class="btn-secondary w-full py-3 justify-center"
          >
            Log ind
          </NuxtLink>
        </div>

      </div>

    </template>
  </div>
</template>

<script setup lang="ts">
import type { FeaturedOfferDto, ServiceRequestDto } from '~/types/api.types'

const { $api } = useNuxtApp()
const route = useRoute()
const authStore = useAuthStore()
const offerId = route.params.offerId as string

// Load the specific offer from the featured feed
const { data: allOffers } = await useAsyncData('featured-offers-detail', () =>
  $api<FeaturedOfferDto[]>('/featured-offers'))

const offer = computed(() =>
  allOffers.value?.find(o => o.offerId === offerId) ?? null)

// Booking form
const bookForm = reactive({ date: '', time: '', city: '' })
const booking = ref(false)
const booked = ref(false)
const bookError = ref('')
const createdRequestId = ref('')

const minDate = new Date().toISOString().split('T')[0]

const danishCities = [
  'København', 'Aarhus', 'Odense', 'Aalborg', 'Esbjerg',
  'Randers', 'Kolding', 'Vejle', 'Horsens', 'Helsingør',
  'Roskilde', 'Silkeborg', 'Næstved', 'Fredericia', 'Herning',
]

// Pre-fill city from the offer
watchEffect(() => {
  if (offer.value && !bookForm.city)
    bookForm.city = offer.value.city
})

async function bookNow() {
  if (!bookForm.date || !bookForm.time || !bookForm.city) return
  bookError.value = ''
  booking.value = true
  try {
    // 1. Create a service request for this category + city
    const request = await $api<ServiceRequestDto>('/requests', {
      method: 'POST',
      body: {
        categoryId: null,
        freeTextDescription: offer.value?.providerMessage ?? offer.value?.categoryName,
        requestedDate: bookForm.date,
        requestedTime: bookForm.time + ':00',
        city: bookForm.city,
      },
    })
    createdRequestId.value = request.id
    booked.value = true
  } catch (e: unknown) {
    const err = e as { data?: { error?: string; errors?: { message: string }[] } }
    bookError.value = err?.data?.error ?? err?.data?.errors?.[0]?.message ?? 'Noget gik galt.'
  } finally {
    booking.value = false
  }
}

function timeAgo(iso: string): string {
  const diff = Date.now() - new Date(iso).getTime()
  const h = Math.floor(diff / 3_600_000)
  if (h < 1) return 'lige nu'
  if (h < 24) return `${h}t siden`
  return `${Math.floor(h / 24)}d siden`
}
</script>
